using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalModels
{
    public class GenericFF_Model
    {
        [Key]
        [StringLength(255)]
        public string FF_Name { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "No Location Path")]
        public string Location_path { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string FK_Father { get; set; }

    }
}
