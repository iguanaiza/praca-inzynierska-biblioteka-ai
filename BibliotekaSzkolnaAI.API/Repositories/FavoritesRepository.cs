using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoritesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<FavoriteBook> Items, int TotalCount)> GetFavoritesAsync(string userId, FavoriteQueryParams queryParams)
        {
            IQueryable<FavoriteBook> query = _context.FavoriteBooks
                .AsNoTracking()
                .Where(fb => fb.UserId == userId)
                .Include(fb => fb.Book)
                    .ThenInclude(b => b.BookAuthor);

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var searchTerm = queryParams.SearchTerm.ToLower();
                query = query.Where(fb =>
                    fb.Book.Title.ToLower().Contains(searchTerm) ||
                    (fb.Book.BookAuthor.Name + " " + fb.Book.BookAuthor.Surname).ToLower().Contains(searchTerm)
                );
            }

            query = query.OrderByDescending(fb => fb.AddedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> IsFavoriteAsync(string userId, int bookId)
        {
            return await _context.FavoriteBooks
                .AnyAsync(f => f.UserId == userId && f.BookId == bookId);
        }

        public async Task<List<int>> GetFavoriteBookIdsAsync(string userId)
        {
            return await _context.FavoriteBooks
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Select(f => f.BookId)
                .ToListAsync();
        }

        public async Task AddAsync(FavoriteBook favorite)
        {
            await _context.FavoriteBooks.AddAsync(favorite);
        }

        public async Task RemoveAsync(FavoriteBook favorite)
        {
            _context.FavoriteBooks.Remove(favorite);
            await Task.CompletedTask;
        }

        public async Task<FavoriteBook?> GetByBookIdAsync(string userId, int bookId)
        {
            return await _context.FavoriteBooks
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
