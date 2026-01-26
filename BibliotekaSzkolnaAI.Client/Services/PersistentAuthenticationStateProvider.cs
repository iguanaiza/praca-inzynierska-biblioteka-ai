using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.Client.Services
{
    public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly Task<AuthenticationState> _unauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        private readonly PersistentComponentState _persistence;
        private AuthenticationState? _authenticationState;

        public PersistentAuthenticationStateProvider(PersistentComponentState persistence)
        {
            _persistence = persistence;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_authenticationState is null)
            {
                if (!_persistence.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
                {
                    return _unauthenticatedTask;
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.GivenName, userInfo.FirstName)
            };

                foreach (var role in userInfo.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, nameof(PersistentAuthenticationStateProvider));
                var principal = new ClaimsPrincipal(identity);
                _authenticationState = new AuthenticationState(principal);
            }

            return Task.FromResult(_authenticationState);
        }
    }
}
