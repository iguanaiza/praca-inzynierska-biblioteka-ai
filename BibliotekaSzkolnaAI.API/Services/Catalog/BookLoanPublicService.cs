using AutoMapper;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;

namespace BibliotekaSzkolnaAI.API.Services.Catalog
{
    public class BookLoanPublicService(ILoansRepository loansRepo, IMapper mapper) : IBookLoanPublicService
    {
        private const decimal DailyPenaltyRate = 1.00m;

        public async Task<List<LoanGetDto>> GetMyLoansAsync(string userId)
        {
            var loans = await loansRepo.GetLoansAsync(userId, null);
            var dtos = mapper.Map<List<LoanGetDto>>(loans);

            foreach (var dto in dtos)
            {
                dto.PenaltyAmount = CalculatePenaltyDto(dto.DueDate, dto.Status, dto.PenaltyAmount);
            }
            return dtos;
        }

        public async Task<bool> ProlongLoanAsync(string userId, int loanId)
        {
            var loan = await loansRepo.GetByIdAsync(loanId);
            if (loan == null || loan.UserId != userId) return false;

            if (loan.WasProlonged || loan.Status != LoanStatus.Active) return false;

            if (loan.DueDate.HasValue && loan.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                loan.Status = LoanStatus.Overdue; 
                await loansRepo.SaveChangesAsync();
                return false;
            }

            loan.DueDate = loan.DueDate?.AddMonths(1);
            loan.WasProlonged = true;
            return await loansRepo.SaveChangesAsync();
        }

        public async Task<bool> RequestReturnAsync(string userId, int loanId)
        {
            var loan = await loansRepo.GetByIdAsync(loanId);
            if (loan == null || loan.UserId != userId) return false;

            if (loan.Status != LoanStatus.Active && loan.Status != LoanStatus.Overdue) return false;

            loan.PenaltyAmount = CalculatePenaltyEntity(loan);

            loan.Status = LoanStatus.PendingReturn;
            loan.ReturnDate = DateTime.UtcNow;

            return await loansRepo.SaveChangesAsync();
        }

        private decimal CalculatePenaltyDto(DateTime? dueDate, LoanStatus status, decimal storedPenalty)
        {
            if (status == LoanStatus.Returned || status == LoanStatus.PendingReturn) return storedPenalty;
            var today = DateTime.UtcNow.Date;
            var due = dueDate?.Date;
            if (due == null || due >= today) return 0;
            return (today - due.Value).Days * DailyPenaltyRate;
        }

        private decimal CalculatePenaltyEntity(BookLoan loan)
        {
            var today = DateTime.UtcNow.Date;
            var dueDate = loan.DueDate?.Date;
            if (dueDate == null || dueDate >= today) return 0;
            return (today - dueDate.Value).Days * DailyPenaltyRate;
        }
    }
}
