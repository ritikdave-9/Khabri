using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class UserKeyword
    {
        public Guid UserID { get; set; }        
        public User User { get; set; }   
        public Guid KeywordID { get; set; }     
        public Keyword Keyword { get; set; }   
        public DateTime CreatedAt { get; set; }
    }

}
