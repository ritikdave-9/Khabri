using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class NewsKeyword
    {
        public Guid NewsID { get; set; }     
        public Guid KeywordID { get; set; }   
        public DateTime CreatedAt { get; set; }
        public virtual News News { get; set; }
        public virtual Keyword Keyword { get; set; }
    }
}
