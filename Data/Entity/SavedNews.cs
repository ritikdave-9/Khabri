using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class SavedNews
    {
        public Guid UserId { get; set; } 
        public User User { get; set; }

        public Guid NewsId { get; set; } 
        public News News { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
