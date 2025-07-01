using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Entity
{
    public class Category
    {
        [Key]
        [Required]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation property
        [JsonIgnore]

        public virtual ICollection<News> NewsCategories { get; set; } = new HashSet<News>();
        [JsonIgnore]

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new HashSet<UserSubscription>();

    }
}
