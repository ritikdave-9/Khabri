using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual User User { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryID { get; set; }
        public virtual Category? Category { get; set; }

        [ForeignKey(nameof(Keyword))]
        public int? KeywordID { get; set; }
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