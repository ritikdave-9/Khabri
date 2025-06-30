using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class    NewsSourceCreateRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        [Url]
        public string BaseURL { get; set; }

        [Required]
        public NewsSourceMappingFieldRequestDto MappingField { get; set; }

        public NewsSourceTokenDto Token { get; set; }
    }
}
