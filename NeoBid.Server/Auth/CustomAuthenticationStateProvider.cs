using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace NeoBid.Server.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private AuthenticationService authenticationService;

        public CustomAuthenticationStateProvider(AuthenticationService service)
        {
            authenticationService = service;

            service.UserChanged += (newUser) =>
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            };
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var claimsPrincipal = await authenticationService.GetClaimsPrincipal();

            return new AuthenticationState(claimsPrincipal);
        }
    }
}