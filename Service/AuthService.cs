using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repository.Interfaces;
using Data.Entity;
using Common.Utils;
using Service.Interfaces;
using Common.Dtos;
namespace Service
{
 

    
        public class AuthService : IAuthService
        {
        IBaseRepository<User> _baseRepositor;

            public AuthService(IBaseRepository<User> baseRepository)
            {
            _baseRepositor = baseRepository;
            }

        public async Task<LoginResponseDto> ValidateUserAsync(string email, string password)
        {
            var user = await _baseRepositor.FindFirstAsync(user => user.Email == email);

            if (user == null)
                return null;

            string hashedInputPassword = PasswordHasher.HashPassword(password);

            if (user.Password != hashedInputPassword)
                return null;

            return new LoginResponseDto
            {
                FirstName=user.FirstName,
                UserID = user.UserID,
                Role = user.Role
            };
        }



    }


}
