using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalModels
{
   public class PDFModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Folder_name { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "No Location Path")]
        public string Location_path { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string FK_suborder { get; set; }
    }
}
