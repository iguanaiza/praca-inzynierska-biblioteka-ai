using AutoMapper;
using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users;
using Microsoft.AspNetCore.Identity;

namespace BibliotekaSzkolnaAI.API.Services.Management
{
    public class UserManagementService(
        IUserRepository userRepo,
        ILoansRepository loansRepo,
        UserManager<ApplicationUser> userManager,
        IMapper mapper) : IUserManagementService
        {
        public async Task<PagedResult<UserForListDto>> GetUsersAsync(int page, int pageSize, string? search)
        {
            var (users, totalCount) = await userRepo.GetUsersAsync(page, pageSize, search);
            var dtos = mapper.Map<List<UserForListDto>>(users);

            foreach (var dto in dtos)
            {
                var userEntity = users.First(u => u.Id == dto.Id);

                if (userEntity.BookLoans != null && userEntity.BookLoans.Any())
                {

                    dto.FineAmount = userEntity.BookLoans.Sum(l =>
                        CalculatePenalty(l.DueDate, l.Status, l.PenaltyAmount, l.ReturnDate));
                }
                else
                {
                    dto.FineAmount = 0;
                }
            }

            return new PagedResult<UserForListDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<UserDetailedDto?> GetUserDetailsAsync(string userId)
        {
            var user = await userRepo.GetUserByIdAsync(userId);
            if (user == null) return null;

            var dto = mapper.Map<UserDetailedDto>(user);
            dto.Roles = (await userRepo.GetUserRolesAsync(user)).ToList();

            decimal totalUserFine = 0;

            if (dto.Loans != null)
            {
                foreach (var loan in dto.Loans)
                {
                    decimal currentLoanPenalty = CalculatePenalty(loan.DueDate, loan.Status, loan.PenaltyAmount, loan.ReturnDate);
                    loan.PenaltyAmount = currentLoanPenalty;
                    totalUserFine += currentLoanPenalty;
                }

                dto.Loans = dto.Loans.OrderByDescending(l => l.BorrowDate).ToList();
            }
            dto.FineAmount = totalUserFine;
            return dto;
        }

        public async Task<string> CreateUserAsync(UserCreateDto dto)
        {
            if (await userRepo.IsPeselTakenAsync(dto.Pesel, ignoreUserId: ""))
            {
                throw new InvalidOperationException($"PESEL {dto.Pesel} jest już zajęty.");
            }

            var user = mapper.Map<ApplicationUser>(dto);

            user.LibraryId = new Random().Next(10000, 99999);
            user.DateAdded = DateTime.UtcNow;
            user.FineAmount = 0;
            user.IsDeleted = false;

            var result = await userRepo.CreateUserAsync(user, dto.Password, dto.Role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Błąd tworzenia: {errors}");
            }

            return user.Id;
        }

        public async Task UpdateUserAsync(string userId, UserEditDto dto)
        {
            var user = await userRepo.GetUserByIdForEditAsync(userId);
            if (user == null) throw new KeyNotFoundException("Użytkownik nie istnieje.");

            if (await userRepo.IsPeselTakenAsync(dto.Pesel, userId))
            {
                throw new InvalidOperationException($"PESEL {dto.Pesel} jest zajęty.");
            }

            mapper.Map(dto, user);
            user.DateModified = DateTime.UtcNow;

            if (!string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailResult = await userManager.SetEmailAsync(user, dto.Email);
                if (!emailResult.Succeeded) throw new InvalidOperationException("Błąd zmiany e-maila.");
                await userManager.SetUserNameAsync(user, dto.Email);
            }

            await userRepo.UpdateUserAsync(user);
        }

        public async Task SoftDeleteUserAsync(string userId)
        {
            var user = await userRepo.GetUserByIdForEditAsync(userId);
            if (user == null) throw new KeyNotFoundException("Użytkownik nie istnieje.");

            if (user.IsDeleted) throw new InvalidOperationException("Użytkownik już jest w koszu.");

            if (await userRepo.HasActiveLoansAsync(userId))
            {
                throw new InvalidOperationException("Nie można usunąć użytkownika, który posiada nieoddane książki.");
            }

            if (user.FineAmount > 0)
            {
                throw new InvalidOperationException($"Użytkownik ma nieopłaconą karę w wysokości {user.FineAmount} PLN. Najpierw ureguluj należności.");
            }

            user.IsDeleted = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.LockoutEnabled = true;
            user.DateModified = DateTime.UtcNow;

            await userRepo.UpdateUserAsync(user);
        }

        public async Task HardDeleteUserAsync(string userId)
        {
            var user = await userRepo.GetUserByIdForEditAsync(userId);
            if (user == null) throw new KeyNotFoundException("Użytkownik nie istnieje.");

            if (!user.IsDeleted)
            {
                throw new InvalidOperationException("Użytkownik musi najpierw trafić do kosza.");
            }

            if (await userRepo.HasActiveLoansAsync(userId))
            {
                throw new InvalidOperationException("Użytkownik posiada nieoddane książki.");
            }

            if (user.FineAmount > 0)
            {
                throw new InvalidOperationException($"Użytkownik ma nieopłaconą karę ({user.FineAmount} PLN).");
            }

            var result = await userRepo.DeleteUserAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Nie udało się usunąć.");
            }
        }

        private decimal CalculatePenalty(DateTime? dueDate, LoanStatus status, decimal storedPenalty, DateTime? returnDate)
        {
            const decimal DailyPenaltyRate = 1.00m;

            if (status == LoanStatus.Returned && returnDate.HasValue)
            {
                if (storedPenalty > 0) return storedPenalty;

                if (dueDate.HasValue && returnDate.Value.Date > dueDate.Value.Date)
                {
                    var daysLate = (returnDate.Value.Date - dueDate.Value.Date).Days;
                    return daysLate * DailyPenaltyRate;
                }

                return 0;
            }

            if (status == LoanStatus.PendingReturn)
            {
                return storedPenalty;
            }

            var today = DateTime.UtcNow.Date;
            var due = dueDate?.Date;

            if (due == null || due >= today) return 0;

            var daysOverdue = (today - due.Value).Days;
            return daysOverdue * DailyPenaltyRate;
        }
    }
}
