using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class NewsSource
    {
        public Guid NewsSourceID { get; set; }
        public string Name { get; set; }
        public string BaseURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<NewsSourceToken> NewsSourceTokens { get; set; } = new List<NewsSourceToken>();
    }
}
