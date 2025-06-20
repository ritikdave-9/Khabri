using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class Category
    {
        public Guid CategoryID { get; set; }     
        public string CategoryName { get; set; }   
        public string Slug { get; set; }       
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
