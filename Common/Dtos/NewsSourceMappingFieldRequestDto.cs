using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class NewsSourceMappingFieldRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [Required]
        public string NewsListKeyString { get; set; } = "data";

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(5000)]
        public string Content { get; set; }

        [MaxLength(200)]
        public string Source { get; set; }

        [MaxLength(200)]
        public string Author { get; set; }

        [Required]
        public string PublishedAt { get; set; }

        public string Keywords { get; set; }

        public string Category { get; set; }

        [MaxLength(10)]
        public string Language { get; set; }
    }

}
