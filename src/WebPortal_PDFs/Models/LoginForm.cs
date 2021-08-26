using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortal_PDFs.Models
{
    public class LoginForm
    {
        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be then 20")]
        public string Username { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
