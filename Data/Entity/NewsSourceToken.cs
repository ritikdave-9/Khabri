﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entity
{
    public class NewsSourceToken
    {
        [Key]
        [Required]
        public int NewsSourceTokenID { get; set; }

        [MaxLength(500)]
        public string Token { get; set; }

        [MaxLength(100)]
        public string TokenKeyString {  get; set; }
                
        [Required]
        public DateTime CreatedAt { get; set; }

    }
}
