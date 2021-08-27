using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortal_PDFs.Models
{
    public class CurrentUser // Holds the current user details
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
}
