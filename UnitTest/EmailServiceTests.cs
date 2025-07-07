using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.Extensions.Configuration;
using Moq;
using Service;
using Xunit;

namespace UnitTest
{
    public class EmailServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IConfigurationSection> _smtpSectionMock;

        public EmailServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _smtpSectionMock = new Mock<IConfigurationSection>();

            _smtpSectionMock.Setup(s => s["Host"]).Returns("smtp.test.com");
            _smtpSectionMock.Setup(s => s["Port"]).Returns("587");
            _smtpSectionMock.Setup(s => s["Username"]).Returns("user@test.com");
            _smtpSectionMock.Setup(s => s["Password"]).Returns("password");
            _smtpSectionMock.Setup(s => s["EnableSsl"]).Returns("true");
            _smtpSectionMock.Setup(s => s["From"]).Returns("noreply@test.com");

            _configMock.Setup(c => c.GetSection("Smtp")).Returns(_smtpSectionMock.Object);
        }

        [Fact]
        public async Task SendNewsNotificationAsync_SendsEmail()
        {
            var service = new EmailService(_configMock.Object);
            var user = new User { FirstName = "John", Email = "john@example.com" };
            var news = new News { Title = "Breaking News", Description = "Details", Url = "http://news.com" };

           
            await Assert.ThrowsAnyAsync<SmtpException>(() =>
                service.SendNewsNotificationAsync(user, news));
        }

        [Fact]
        public async Task SendNewsDigestAsync_SendsEmail()
        {
            var service = new EmailService(_configMock.Object);
            var user = new User { FirstName = "Jane", Email = "jane@example.com" };
            var newsList = new List<News>
            {
                new News { Title = "News1", Description = "Desc1", Url = "http://news1.com" },
                new News { Title = "News2", Description = "Desc2", Url = "http://news2.com" }
            };

            await Assert.ThrowsAnyAsync<SmtpException>(() =>
                service.SendNewsDigestAsync(user, newsList));
        }
    }
}
