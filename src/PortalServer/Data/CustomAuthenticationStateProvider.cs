
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
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

        public NavigationManager NavigationManager { get; }

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorageService, NavigationManager navigationManager)
        {
            _sessionStorageService = sessionStorageService;
            NavigationManager = navigationManager;
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var username = await _sessionStorageService.GetItemAsync<string>("Username");
            var role_token = await _sessionStorageService.GetItemAsync<bool>("Role");
            string role;
            ClaimsIdentity identity; 

            if(username != null)
            {
                if (role_token) { role = "User"; } else { role = "Admin"; };

                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                }, "apiauth_type");

            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(user));

        }


        public async Task SetUserAsAuthenticated(string username, bool role_token)
        {
            await _sessionStorageService.SetItemAsStringAsync("Username", username);

            string role = "";
            if (role_token) { role = "Admin"; } else { role = "User";  }; 
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username),new Claim( ClaimTypes.Role, role) }, "Cookies");

            var user_cookie = new ClaimsPrincipal(identity);

            // Serializze
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user_cookie)));
        }


        public async Task LogoutUserFromAuthentication()
        {
            var identity = new ClaimsIdentity();
            var user_cookie = new ClaimsPrincipal(identity);

            await _sessionStorageService.RemoveItemAsync("Username");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user_cookie)));
            NavigationManager.NavigateTo("/");

        }
    }
}
