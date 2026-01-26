using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class LoansRepository : ILoansRepository
    {
        private readonly ApplicationDbContext _context;

        public LoansRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookLoan>> GetLoansAsync(string? userId, List<LoanStatus> statuses)
        {
            var today = DateTime.Now.Date;

            await _context.BookLoans
                    .Where(l => l.Status == LoanStatus.Active && l.DueDate < today && l.ReturnDate == null)
                    .ExecuteUpdateAsync(s => s.SetProperty(b => b.Status, LoanStatus.Overdue));

            var query = _context.BookLoans
                .AsNoTracking()
                .Include(l => l.BookCopy.Book.BookAuthor)
                .Include(l => l.BookCopy.Book.BookPublisher)
                .Include(l => l.User) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(l => l.UserId == userId);
            }

            if (statuses != null && statuses.Any())
            {
                query = query.Where(l => statuses.Contains(l.Status));
            }

            return await query.OrderByDescending(l => l.BorrowDate).ToListAsync();
        }

        public async Task<BookLoan?> GetByIdAsync(int id)
        {
            return await _context.BookLoans
                .Include(l => l.BookCopy)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<BookLoan>> GetLoansWithPenaltiesAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.BookLoans
                .AsNoTracking()
                .Include(l => l.User)             
                .Include(l => l.BookCopy)        
                    .ThenInclude(bc => bc.Book)  
                .Where(l =>
                   
                    l.PenaltyAmount > 0 || (l.ReturnDate == null && l.DueDate < today)
                )
                .OrderByDescending(l => l.DueDate)
                .ToListAsync();
        }
    }
}
