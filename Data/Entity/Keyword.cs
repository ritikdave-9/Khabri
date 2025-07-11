﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Entity
{
    public class Keyword
    {
        [Key]
        [Required]
        public int KeywordID { get; set; }

        [Required]
        [MaxLength(100)]
        public string KeywordText { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]

        public virtual ICollection<News> News { get; set; } = new HashSet<News>();
        [JsonIgnore]

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
        [JsonIgnore]

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new HashSet<UserSubscription>();

    }
}
