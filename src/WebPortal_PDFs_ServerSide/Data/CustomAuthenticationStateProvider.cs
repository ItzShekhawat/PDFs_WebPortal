using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebPortal_PDFs_ServerSide
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {   
       
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Karni@gmail.com"),
            },  "apiauth_type");

            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        
        }


        public void SetUserAdAuthenticated(string username)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "apiauth_type");

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
