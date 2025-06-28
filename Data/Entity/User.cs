using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace Data.Entity
{
    public class User
    {
        [Key]
        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)] 
        public string Password { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        public virtual ICollection<Keyword> SubscribedKeywords { get; set; } = new HashSet<Keyword>();
        public virtual ICollection<News> SavedNews { get; set; } = new HashSet<News>();
    }
}
