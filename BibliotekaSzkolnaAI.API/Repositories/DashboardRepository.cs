using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var vm = new DashboardViewModel();
            var today = DateTime.UtcNow.Date;
            var sixMonthsAgo = today.AddMonths(-6);

            var loansQuery = _context.BookLoans.AsNoTracking();

            vm.PendingLoanApprovalsCount = await loansQuery.CountAsync(l => l.Status == LoanStatus.PendingApproval);
            vm.PendingReturnApprovalsCount = await loansQuery.CountAsync(l => l.Status == LoanStatus.PendingReturn);
            vm.ActiveLoansCount = await loansQuery.CountAsync(l => l.Status == LoanStatus.Active && (l.DueDate == null || l.DueDate >= today));
            vm.OverdueLoansCount = await loansQuery.CountAsync(l => l.Status == LoanStatus.Overdue || (l.Status == LoanStatus.Active && l.DueDate < today));

            vm.LoansPerMonth = await loansQuery
                .Where(l => l.BorrowDate >= sixMonthsAgo)
                .GroupBy(l => new { l.BorrowDate.Year, l.BorrowDate.Month })
                .Select(g => new MonthlyLoanStatDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    MonthName = ""
                })
                .ToListAsync();

            vm.RecommendedBooks = await _context.Books
                .AsNoTracking()
                .Where(b => b.BookBookSpecialTags.Any(t => t.BookSpecialTag.Title == "Polecane") && b.IsVisible && !b.IsDeleted)
                .Include(b => b.BookAuthor)
                .Include(b => b.BookPublisher)
                .Select(b => new BookGetBaseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    BookAuthor = b.BookAuthor.Surname + " " + b.BookAuthor.Name,
                    BookPublisher = b.BookPublisher.Name,
                    AvailableCopyCount = b.BookCopies.Count(c => c.Available)
                })
                .Take(5)
                .ToListAsync();

            return vm;
        }
    }
}
