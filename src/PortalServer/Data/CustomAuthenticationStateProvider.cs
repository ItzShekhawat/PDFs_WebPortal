
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalServer.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorageService)
        {
            _sessionStorageService = sessionStorageService;
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var username = await _sessionStorageService.GetItemAsync<string>("Username");
            ClaimsIdentity identity; 

            if(username != null)
            { 
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username ),
                }, "apiauth_type");

            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(user));

        }


        public void SetUserAdAuthenticated(string username)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "apiauth_type");

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
