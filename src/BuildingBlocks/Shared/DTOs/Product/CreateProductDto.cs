using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Product
{
    public class CreateProductDto : CreateOrUpdateProductDto
    {
        [Required]
        [MaxLength(150, ErrorMessage = "Maximum length for Product No is 150 characters.")]
        public string No { get; set; }
    }
}
