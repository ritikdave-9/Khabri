using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Enums;

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

        [Required]
        public NewsSourceStatus Status { get; set; } = NewsSourceStatus.Active;
        [JsonIgnore]

        public virtual NewsSourceToken NewsSourceToken { get; set; }
        [JsonIgnore]

        public virtual NewsSourceMappingField NewsSourceMappingField { get; set; }
    }
}
