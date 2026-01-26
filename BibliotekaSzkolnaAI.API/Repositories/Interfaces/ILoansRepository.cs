using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.Shared.Common;

namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface ILoansRepository
    {
        Task<List<BookLoan>> GetLoansAsync(string? userId, List<LoanStatus> statuses);

        Task<BookLoan?> GetByIdAsync(int id);

        Task<bool> SaveChangesAsync();
        Task<List<BookLoan>> GetLoansWithPenaltiesAsync();
    }
}
