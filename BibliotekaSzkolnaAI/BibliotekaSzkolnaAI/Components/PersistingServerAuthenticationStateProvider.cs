using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.Server.Components
{
    public class PersistingServerAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
    {
        private readonly PersistentComponentState _persistence;
        private readonly PersistingComponentStateSubscription _subscription;
        private Task<AuthenticationState>? _authenticationStateTask;

        private readonly ILogger<PersistingServerAuthenticationStateProvider> _logger;

        public PersistingServerAuthenticationStateProvider(
            PersistentComponentState persistentComponentState,
            ILogger<PersistingServerAuthenticationStateProvider> logger)
        {
            _persistence = persistentComponentState;
            _logger = logger;
            _logger.LogInformation("PersistingServerAuthenticationStateProvider został skonstruowany.");

            _subscription = _persistence.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        private async Task OnPersistingAsync()
        {
            _logger.LogInformation(">>> Rozpoczęto OnPersistingAsync (próba zapisu stanu).");

            _authenticationStateTask ??= GetAuthenticationStateAsync();
            var authState = await _authenticationStateTask;
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation(">>> Użytkownik JEST uwierzytelniony. Próba zapisu UserInfo.");

                var userInfo = new UserInfo
                {
                    UserId = user.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    Email = user.FindFirstValue(ClaimTypes.Email)!,
                    FirstName = user.FindFirstValue(ClaimTypes.GivenName)!,
                    Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                };

                _persistence.PersistAsJson(nameof(UserInfo), userInfo);
            }
            else
            {
                _logger.LogWarning(">>> Użytkownik NIE JEST uwierzytelniony (stan anonimowy). Nic nie zapisano.");
            }
        }

        public void Dispose()
        {
            _subscription.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}