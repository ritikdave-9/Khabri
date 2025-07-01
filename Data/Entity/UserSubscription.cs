using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Data.Entity;

namespace Data.Entity
{
    public class UserSubscription
    {
        [Key]
        public int UserSubscriptionID { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        [JsonIgnore]

        public virtual User User { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryID { get; set; }
        [JsonIgnore]

        public virtual Category? Category { get; set; }

        [ForeignKey(nameof(Keyword))]
        public int? KeywordID { get; set; }
        [JsonIgnore]

        public virtual Keyword? Keyword { get; set; }

        [Required]
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public bool IsValid
        {
            get
            {
                return (CategoryID.HasValue || KeywordID.HasValue) && UserID > 0;
            }
        }
    }
}