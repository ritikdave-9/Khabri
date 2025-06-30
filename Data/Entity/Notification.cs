using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(News))]
        public int? NewsID { get; set; }
        public virtual News? News { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsSeen { get; set; } = false;

        
    }
}