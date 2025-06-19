using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class Keyword
    {
        public Guid KeywordID { get; set; }  

        public string KeywordText { get; set; }  

        public DateTime CreatedAt { get; set; }
    }

}
