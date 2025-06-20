using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dtos;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> ValidateUserAsync(string email, string password);
    }
}
