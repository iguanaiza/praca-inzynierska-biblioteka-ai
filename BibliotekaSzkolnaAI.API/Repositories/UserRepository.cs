using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class UserRepository(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context) : IUserRepository
    {
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<(List<ApplicationUser> Items, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = userManager.Users
                        .Include(u => u.BookLoans)
                        .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.LibraryId.ToString().Contains(searchTerm) ||
                    u.Pesel.Contains(searchTerm)
                );
            }

            query = query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await userManager.Users
                .AsNoTracking()
                .Include(u => u.BookLoans)
                    .ThenInclude(bl => bl.BookCopy)
                        .ThenInclude(bc => bc.Book)
                            .ThenInclude(b => b.BookAuthor)
                .Include(u => u.BookLoans)
                    .ThenInclude(bl => bl.BookCopy)
                        .ThenInclude(bc => bc.Book)
                            .ThenInclude(b => b.BookPublisher)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> GetUserByIdForEditAsync(string userId)
        {
            return await context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role)
        {
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return result;
            }

            try
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    return roleResult;
                }
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(user);

                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleAssignmentError",
                    Description = $"Użytkownik utworzony, ale wystąpił błąd przy nadawaniu roli '{role}': {ex.Message}"
                });
            }

            return IdentityResult.Success;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> HasActiveLoansAsync(string userId)
        {
            return await context.BookLoans
                .AnyAsync(l => l.UserId == userId &&
                               (l.Status == LoanStatus.Active ||
                                l.Status == LoanStatus.Overdue ||
                                l.Status == LoanStatus.PendingReturn));
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await userManager.DeleteAsync(user);
        }

        public async Task<bool> IsPeselTakenAsync(string pesel, string ignoreUserId)
        {
            return await context.Users
                .AnyAsync(u => u.Pesel == pesel && u.Id != ignoreUserId);
        }
    }
}
