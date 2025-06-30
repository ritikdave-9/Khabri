using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace Common.Dtos
{
    public class LoginResponseDto
    {
        public string FirstName { get; set; }
        public int UserID { get; set; }
        public Role Role { get; set; }
        public string AuthToken { get; set; }
    }
}
