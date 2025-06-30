using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entity
{
    public class NewsSourceMappingField
    {
        [Key]
        [Required]
        public int NewsSourceMappingFieldID { get; set; }

        [Required]
        public string NewsListKeyString { get; set; } = "data";

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Source { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        public string PublishedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Keywords { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Language { get; set; } = string.Empty;
    }
}
