using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;

namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface IBookLoanManagementService
    {
        Task<List<LoanManagementDto>> GetAllLoansAsync(List<LoanStatus> statuses);

        Task<bool> ApproveLoanAsync(int loanId);

        Task<bool> FinalizeReturnAsync(int loanId);
    }
}
