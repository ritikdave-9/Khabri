using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Entity
{
    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        [ForeignKey(nameof(Reporter))]
        public int ReporterID { get; set; }
        [JsonIgnore]

        public virtual User Reporter { get; set; }

        [Required]
        [ForeignKey(nameof(News))]
        public int NewsID { get; set; }
        [JsonIgnore]

        public virtual News News { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; } = false;
    }
}
