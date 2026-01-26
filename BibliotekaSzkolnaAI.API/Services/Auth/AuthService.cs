using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.API.Services.Auth
{
    public class AuthService(
         IUserRepository userRepo,
         TokenService tokenService) : IAuthService
    {
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await userRepo.GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            var isPasswordValid = await userRepo.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            var userRoles = await userRepo.GetUserRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Email!),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.GivenName, user.FirstName)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenString = tokenService.GenerateToken(claims);

            return new LoginResponseDto
            {
                Token = tokenString
            };
        }
    }
}
