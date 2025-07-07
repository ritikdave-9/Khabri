using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Service.Interfaces;
using Data.Entity;
using Data.Repository.Interfaces;
using Common.Dtos;
using Common.Exceptions;
using Common.Utils;
using Moq;
namespace UnitTest
{
    public class AuthServiceTests
    {
        private readonly Mock<IBaseRepository<User>> _userRepoMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly JwtSettings _jwtSettings;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IBaseRepository<User>>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _jwtSettings = new Common.Utils.JwtSettings 
            {
                Secret = "testseddfdsfsdfsdfsdsdfsdfcret1234",
                Issuer = "testissuer",
                Audience = "testaudience",
                ExpiresInMinutes = "60"
            };
            _jwtOptions = Options.Create(_jwtSettings);

            
          _serviceProviderMock
    .Setup(x => x.GetService(typeof(IBaseRepository<User>)))
    .Returns(_userRepoMock.Object);
        }

        [Fact]
        public async Task ValidateUserAsync_UserNotFound_ThrowsAuthException()
        {
            
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                .ReturnsAsync((User)null);

            var service = new AuthService(_serviceProviderMock.Object, _jwtOptions);

            
            await Assert.ThrowsAsync<AuthException>(() => service.ValidateUserAsync("test@example.com", "password"));
        }

        [Fact]
        public async Task ValidateUserAsync_WrongPassword_ThrowsAuthException()
        {
            
            var user = new User { Email = "test@example.com", Password = "hashed", UserID = 1, FirstName = "Test", Role = Common.Enums.Role.User };
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>() ))
                .ReturnsAsync(user);

            var service = new AuthService(_serviceProviderMock.Object, _jwtOptions);

            
            await Assert.ThrowsAsync<AuthException>(() => service.ValidateUserAsync("test@example.com", "wrongpassword"));
        }

        [Fact]
        public async Task ValidateUserAsync_ValidUser_ReturnsLoginResponseDto()
        {
            
            var password = "password";
            var hashedPassword = Common.Utils.PasswordHasher.HashPassword(password);
            var user = new User { Email = "test@example.com", Password = hashedPassword, UserID = 1, FirstName = "Test", Role = Common.Enums.Role.User };
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                .ReturnsAsync(user);

            var service = new AuthService(_serviceProviderMock.Object, _jwtOptions);

            
            var result = await service.ValidateUserAsync("test@example.com", password);

            
            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.UserID, result.UserID);
            Assert.Equal(user.Role.ToString(), result.Role);
            Assert.False(string.IsNullOrEmpty(result.AuthToken));
        }
    }
}