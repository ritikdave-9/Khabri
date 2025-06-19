using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class NewsSourceToken
    {
        public Guid NewsSourceTokenID { get; set; }
        public string Token { get; set; }
        public Guid NewsSourceID { get; set; } 
        public DateTime CreatedAt { get; set; }
        public NewsSource NewsSource { get; set; }
    }
}
