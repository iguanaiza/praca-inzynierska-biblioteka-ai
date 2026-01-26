using BibliotekaSzkolnaAI.API.Data;

using BibliotekaSzkolnaAI.API.Services.Auth;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.API.Controllers.Public
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            var result = await authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized("Niepoprawne dane logowania.");
            }

            return Ok(result);
        }
    }
}
