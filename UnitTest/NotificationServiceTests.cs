using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entity;
using Data.Repository.Interfaces;
using Moq;
using Service;
using Xunit;

namespace UnitTest
{
    public class NotificationServiceTests
    {
        private readonly Mock<IBaseRepository<Notification>> _repoMock;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _repoMock = new Mock<IBaseRepository<Notification>>();
            _service = new NotificationService(_repoMock.Object);
        }

        [Fact]
        public async Task AddNotificationAsync_AddsNotification()
        {
            
            await _service.AddNotificationAsync(1, 2);

            
            _repoMock.Verify(r => r.AddAsync(It.Is<Notification>(
                n => n.UserID == 1 && n.NewsID == 2 && !n.IsSeen)), Times.Once);
        }

        [Fact]
        public async Task GetUserNotificationsAsync_ReturnsNotifications()
        {
            
            var notifications = new List<Notification> { new Notification { UserID = 1 } };
            _repoMock.Setup(r => r.FindAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Notification, object>>>()))
                .ReturnsAsync(notifications);

            
            var result = await _service.GetUserNotificationsAsync(1);

            
            Assert.Single(result);
            Assert.Equal(1, result.First().UserID);
        }

        [Fact]
        public async Task MarkAsSeenAsync_Updates_WhenNotSeen()
        {
            
            var notification = new Notification { NotificationID = 5, IsSeen = false };
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(notification);

            
            await _service.MarkAsSeenAsync(5);

            
            Assert.True(notification.IsSeen);
            _repoMock.Verify(r => r.UpdateAsync(notification), Times.Once);
        }

        [Fact]
        public async Task MarkAsSeenAsync_DoesNothing_WhenAlreadySeen()
        {
            
            var notification = new Notification { NotificationID = 5, IsSeen = true };
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(notification);

            
            await _service.MarkAsSeenAsync(5);

            
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public async Task MarkAsSeenAsync_DoesNothing_WhenNotFound()
        {
            
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Notification)null);

            
            await _service.MarkAsSeenAsync(5);

            
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Never);
        }
    }
}
