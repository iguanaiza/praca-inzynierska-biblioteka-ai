using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication;

namespace BibliotekaSzkolnaAI.API.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}
