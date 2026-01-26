using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;

namespace BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces
{
    public interface IBookLoanPublicService
    {
        Task<List<LoanGetDto>> GetMyLoansAsync(string userId);
        Task<bool> ProlongLoanAsync(string userId, int loanId);
        Task<bool> RequestReturnAsync(string userId, int loanId);
    }
}
