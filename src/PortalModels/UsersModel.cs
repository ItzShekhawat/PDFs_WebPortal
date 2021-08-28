using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalModels
{
    public class UsersModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Full_name { get; set; }

        [Required]
        [StringLength(64)]
        public string Password { get; set; }

        [Required]
        public bool Access_token { get; set; }

        [Required]
        [StringLength(40)]
        public string Department { get; set; }
    }
}
