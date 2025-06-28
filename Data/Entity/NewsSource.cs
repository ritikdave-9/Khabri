using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entity
{
    public class NewsSource
    {
        [Key]
        [Required]
        public int NewsSourceID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        
        [Url]
        public string BaseURL { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public int NewsSourceMappingFieldID { get; set; }
        public int NewsSourceTokenID { get; set; }

        
        public virtual NewsSourceToken NewsSourceToken { get; set; } 
        public virtual NewsSourceMappingField NewsSourceMappingField { get; set; }
    }
}
