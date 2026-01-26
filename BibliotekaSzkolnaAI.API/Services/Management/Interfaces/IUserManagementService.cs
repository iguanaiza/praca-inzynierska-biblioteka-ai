using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users;

namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface IUserManagementService
    {
        Task<PagedResult<UserForListDto>> GetUsersAsync(int page, int pageSize, string? search);
        Task<UserDetailedDto?> GetUserDetailsAsync(string userId);

        Task UpdateUserAsync(string userId, UserEditDto dto);
        Task<string> CreateUserAsync(UserCreateDto dto);
        Task SoftDeleteUserAsync(string userId);
        Task HardDeleteUserAsync(string userId);
    }
}
