using Blazored.LocalStorage;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebPortal_PDFs
{
    public partial class Program
    {
        // This will check if the client has sent an authentication token.
        // In the negative case it will send an error message back. 
        public class AuthorizationMessageHandler : DelegatingHandler
        {
            private readonly ILocalStorageService  _storage;

            public AuthorizationMessageHandler(ILocalStorageService storage)
            {
                _storage = storage;
            }
            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {   

                // Checking if there are any access token in the local storage and is case it is there put it in the header
                if (await _storage.ContainKeyAsync("access_token"))
                {
                    var token = await _storage.GetItemAsStringAsync("access_token");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("Authorization Message Handler Called");
                }

                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}
