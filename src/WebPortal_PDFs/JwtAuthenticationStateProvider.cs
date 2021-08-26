using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebPortal_PDFs
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _storage;

        public JwtAuthenticationStateProvider(ILocalStorageService storage)
        {
            _storage = storage; 
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
             if(await _storage.ContainKeyAsync("Access_token"))
            {
                // User in Logged 
                var tokenAsString = await _storage.GetItemAsStringAsync("access_token");
                var tokenHandler = new JwtSecurityTokenHandler(); // Helps us decode the token 

                var token = tokenHandler.ReadJwtToken(tokenAsString);
                var identity = new ClaimsIdentity(token.Claims, "Bearer"); // Take the claims of the token and create an identity
                var user = new ClaimsPrincipal(identity);
                var authState = new AuthenticationState(user);

                NotifyAuthenticationStateChanged(Task.FromResult(authState));

                return authState;

            }
            return new AuthenticationState(new ClaimsPrincipal()); // Empty Claims --> No Identity and user not logged in
        }
    }
}
