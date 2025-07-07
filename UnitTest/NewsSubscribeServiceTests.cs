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
    public class NewsSubscribeServiceTests
    {
        private readonly Mock<IBaseRepository<UserSubscription>> _repoMock;
        private readonly NewsSubscribeService _service;

        public NewsSubscribeServiceTests()
        {
            _repoMock = new Mock<IBaseRepository<UserSubscription>>();
            _service = new NewsSubscribeService(_repoMock.Object);
        }

        [Fact]
        public async Task SubscribeAsync_Throws_WhenNoCategoryOrKeyword()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.SubscribeAsync(1, null, null));
        }

        [Fact]
        public async Task SubscribeAsync_Throws_WhenAlreadySubscribed()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>()))
                .ReturnsAsync(new UserSubscription());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SubscribeAsync(1, 2, null));
        }

        [Fact]
        public async Task SubscribeAsync_AddsSubscription_WhenNotExists()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>()))
                .ReturnsAsync((UserSubscription)null);

            await _service.SubscribeAsync(1, 2, null);

            _repoMock.Verify(r => r.AddAsync(It.Is<UserSubscription>(
                s => s.UserID == 1 && s.CategoryID == 2 && s.KeywordID == null)), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeAsync_Deletes_WhenExists()
        {
            var sub = new UserSubscription { UserSubscriptionID = 10, UserID = 1, CategoryID = 2, KeywordID = null };
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>()))
                .ReturnsAsync(sub);

            await _service.UnsubscribeAsync(1, 2, null);

            _repoMock.Verify(r => r.DeleteAsync(10), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeAsync_DoesNothing_WhenNotExists()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>()))
                .ReturnsAsync((UserSubscription)null);

            await _service.UnsubscribeAsync(1, 2, null);

            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetUserSubscriptionsAsync_ReturnsSubscriptions()
        {
            var list = new List<UserSubscription> { new UserSubscription { UserID = 1 } };
            _repoMock.Setup(r => r.FindAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, object>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, object>>>()))
                .ReturnsAsync(list);

            var result = await _service.GetUserSubscriptionsAsync(1);

            Assert.Single(result);
            Assert.Equal(1, result.First().UserID);
        }
    }
}
