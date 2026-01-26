using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth");

            // POST /api/auth/login
            group.MapPost("/login", async ([FromBody] LoginRequestDto loginDto, IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.PostAsJsonAsync("api/auth/login", loginDto);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Unauthorized();
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (string.IsNullOrEmpty(loginResponse?.Token))
                {
                    return Results.Problem("API zwróciło pusty token.");
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(loginResponse.Token);

                var identity = new ClaimsIdentity(
                    jwt.Claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role
                );

                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = loginDto.RememberMe,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7),
                    AllowRefresh = true
                };

                authProperties.StoreTokens(new[]
                {
                new AuthenticationToken { Name = "access_token", Value = loginResponse.Token }
            });

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                return Results.Ok();
            });

            // POST /api/auth/logout
            group.MapPost("/logout", async (HttpContext httpContext) =>
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Ok();
            });
        }
    }
}
