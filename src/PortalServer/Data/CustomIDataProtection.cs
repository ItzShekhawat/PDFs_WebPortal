using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PortalServer.Data
{
    public class CustomIDataProtection
    {
        private readonly IDataProtector protector;

        public CustomIDataProtection(IDataProtectionProvider dataProtectionProvider, UniqueCode uniqueCode)
        {
            protector = dataProtectionProvider.CreateProtector(uniqueCode.UrlKey);
        }
        public string Decode(string data)
        {
            return protector.Protect(data);
        }
        public string Encode(string data)
        {
            Console.WriteLine(data + "<--Data ");
            return protector.Unprotect(data);
        }
        
    }

    
}
