using Common.Dtos;
using Common.Exceptions;
using Common.Utils;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service.Interfaces;
namespace Service
{



    public class AuthService : IAuthService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly JwtSettings _jwtSettings;


        public AuthService(IServiceProvider serviceProvider , IOptions<JwtSettings> jwtOptions)
        {
            _serviceProvider = serviceProvider;
            _jwtSettings = jwtOptions.Value;


        }

        public async Task<LoginResponseDto> ValidateUserAsync(string email, string password)
        {
            var userRepo = _serviceProvider.GetRequiredService<IBaseRepository<User>>();
            var user = await userRepo.FindAsync(user => user.Email == email);

            if (user == null)
                throw new AuthException("User does not exist please signup");

            string hashedInputPassword = PasswordHasher.HashPassword(password);

            if (user.Password != hashedInputPassword)
                throw new AuthException("Incorrect password.");

            var AuthToken = JwtTokenHelper.GenerateToken(user.UserID, user.Role.ToString(), user.Email, _jwtSettings);

            return new LoginResponseDto
            {
                FirstName = user.FirstName,
                UserID = user.UserID,
                Role = user.Role.ToString(),
                AuthToken = AuthToken
            };
        }



    }


}
