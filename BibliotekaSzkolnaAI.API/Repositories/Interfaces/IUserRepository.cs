using BibliotekaSzkolnaAI.API.Data;
using Microsoft.AspNetCore.Identity;

namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);

        Task<(List<ApplicationUser> Items, int TotalCount)> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByIdForEditAsync(string userId);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);

        Task<bool> IsPeselTakenAsync(string pesel, string ignoreUserId);

        Task<bool> HasActiveLoansAsync(string userId);
    }
}
