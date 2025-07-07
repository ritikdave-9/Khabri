using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Dtos;
using Common.Exceptions;
using Data.Entity;
using Moq;
using Service;
using Service.Interfaces;
using Xunit;

namespace UnitTest
{
    public class UserServiceTests
    {
        private readonly Mock<IBaseService<User>> _baseServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _baseServiceMock = new Mock<IBaseService<User>>();
            _mapperMock = new Mock<IMapper>();
            _service = new UserService(_baseServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task SignUpAsync_AddsUser_WhenEmailNotExists()
        {
            
            var dto = new UserSignupDto { Email = "test@example.com", Password = "password", FirstName = "Test", LastName = "User" };
            var user = new User { Email = dto.Email };
            _baseServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _baseServiceMock.Setup(s => s.AddAsync(user)).ReturnsAsync(user);

            
            var result = await _service.SignUpAsync(dto);

            
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(Common.Enums.Role.User, result.Role);
            Assert.False(string.IsNullOrEmpty(result.Password));
            _baseServiceMock.Verify(s => s.AddAsync(user), Times.Once);
        }

        [Fact]
        public async Task SignUpAsync_Throws_WhenEmailExists()
        {
            
            var dto = new UserSignupDto { Email = "test@example.com", Password = "password" };
            var existingUser = new User { Email = dto.Email };
            _baseServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(existingUser);

            
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SignUpAsync(dto));
        }

        [Fact]
        public async Task SignUpAsync_ThrowsWrappedException_OnOtherException()
        {
            
            var dto = new UserSignupDto { Email = "test@example.com", Password = "password" };
            _baseServiceMock.Setup(s => s.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ThrowsAsync(new Exception("db error"));

            
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.SignUpAsync(dto));
            Assert.Contains("An error occurred during signup.", ex.Message);
        }
    }
}
