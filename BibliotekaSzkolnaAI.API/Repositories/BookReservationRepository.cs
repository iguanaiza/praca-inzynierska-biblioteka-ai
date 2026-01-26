using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class BookReservationRepository : IBookReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public BookReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookReservationCart>> GetCartItemsEntitiesAsync(string userId)
        {
            return await _context.BookReservationCarts
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsFinalized)
                .Include(c => c.BookCopy)
                    .ThenInclude(bc => bc.Book)
                        .ThenInclude(b => b.BookAuthor)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();
        }

        public async Task<bool> IsCopyInCartAsync(string userId, int bookCopyId)
        {
            return await _context.BookReservationCarts
                .AnyAsync(c => c.UserId == userId && c.BookCopyId == bookCopyId && !c.IsFinalized);
        }

        public async Task<bool> IsAnyCopyOfBookInCartAsync(string userId, int bookId)
        {
            return await _context.BookReservationCarts
                .Include(c => c.BookCopy) 
                .AnyAsync(c => c.UserId == userId && c.BookCopy.BookId == bookId && !c.IsFinalized);
        }

        public async Task<BookCopy?> GetBookCopyAsync(int bookCopyId)
        {
            return await _context.BookCopies
                .AsNoTracking()
                .Include(c => c.Book)
                .FirstOrDefaultAsync(bc => bc.Id == bookCopyId);
        }

        public async Task<BookReservationCart?> GetCartItemAsync(string userId, int cartItemId)
        {
            return await _context.BookReservationCarts
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId && !c.IsFinalized);
        }

        public async Task AddToCartAsync(BookReservationCart cartItem)
        {
            await _context.BookReservationCarts.AddAsync(cartItem);
        }

        public async Task RemoveFromCartAsync(BookReservationCart cartItem)
        {
            _context.BookReservationCarts.Remove(cartItem);
            await Task.CompletedTask;
        }

        public async Task ClearCartAsync(string userId)
        {
            var items = await _context.BookReservationCarts
                .Where(c => c.UserId == userId && !c.IsFinalized)
                .ToListAsync();

            if (items.Any())
            {
                _context.BookReservationCarts.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ProcessCheckoutAsync(List<BookReservationCart> itemsToRemove, List<BookLoan> loansToAdd)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var loan in loansToAdd)
                {
                    var copy = await _context.BookCopies.FindAsync(loan.BookCopyId);
                    if (copy != null)
                    {
                        copy.Available = false;
                    }
                }

                await _context.BookLoans.AddRangeAsync(loansToAdd);

                foreach (var item in itemsToRemove)
                {
                    var itemToDelete = new BookReservationCart { Id = item.Id };
                    _context.BookReservationCarts.Attach(itemToDelete);
                    _context.BookReservationCarts.Remove(itemToDelete);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd finalizacji: {ex.ToString()}");
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
