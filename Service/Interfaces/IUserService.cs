using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dtos;
using Data.Entity;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<User> SignUpAsync(UserSignupDto dto);
    }
}
