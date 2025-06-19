using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repository.Interfaces;
using Data.Entity;
using Common.Utils;
using Service.Interfaces;
namespace Service
{
 

    
        public class AuthService : IAuthService
        {
        IBaseRepository<User> _baseRepositor;

            public AuthService(IBaseRepository<User> baseRepository)
            {
            _baseRepositor = baseRepository;
            }

            public async Task<bool> ValidateUserAsync(string email, string password)
            {
                var user = await _baseRepositor.FindFirstAsync(user=>user.Email==email);

                if (user == null)
                    return false;

                string hashedInputPassword = PasswordHasher.HashPassword(password);
                return user.Password == hashedInputPassword;
            }

           
        }
    

}
