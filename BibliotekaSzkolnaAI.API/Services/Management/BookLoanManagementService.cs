using AutoMapper;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;

namespace BibliotekaSzkolnaAI.API.Services.Management
{
    public class BookLoanManagementService(ILoansRepository loansRepo, IMapper mapper) : IBookLoanManagementService
    {
        public async Task<List<LoanManagementDto>> GetAllLoansAsync(List<LoanStatus> statuses)
        {
            var loans = await loansRepo.GetLoansAsync(userId: null, statuses: statuses);
            return mapper.Map<List<LoanManagementDto>>(loans);
        }

        public async Task<bool> ApproveLoanAsync(int loanId)
        {
            var loan = await loansRepo.GetByIdAsync(loanId);
            if (loan == null) return false;

            if (loan.Status != LoanStatus.PendingApproval) return false;

            loan.Status = LoanStatus.Active;
            loan.BorrowDate = DateTime.UtcNow;

            return await loansRepo.SaveChangesAsync();
        }

        public async Task<bool> FinalizeReturnAsync(int loanId)
        {
            var loan = await loansRepo.GetByIdAsync(loanId);
            if (loan == null) return false;

            if (loan.Status == LoanStatus.Returned) return false;

            loan.Status = LoanStatus.Returned;

            if (loan.ReturnDate == null)
            {
                loan.ReturnDate = DateTime.UtcNow;
            }

            if (loan.BookCopy != null)
            {
                loan.BookCopy.Available = true;
            }

            return await loansRepo.SaveChangesAsync();
        }
    }
}
