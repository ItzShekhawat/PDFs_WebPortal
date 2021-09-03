using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalModels
{
    public class PDF_FileModel
    {
        [Key]
        [StringLength(255)]
        public string FF_Name { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "No Location Path")]
        public string Location_path { get; set; }

        [Required]
        public int FK_Father { get; set; }
    }
}
