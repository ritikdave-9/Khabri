using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Data.Repository.Interfaces;
using Moq;
using Service;
using Xunit;

namespace UnitTest
{
    public class PersonalizedNewsServiceTests
    {
        private readonly Mock<IBaseRepository<User>> _userRepoMock;
        private readonly Mock<IBaseRepository<News>> _newsRepoMock;
        private readonly Mock<IBaseRepository<UserSubscription>> _subscriptionRepoMock;
        private readonly PersonalizedNewsService _service;

        public PersonalizedNewsServiceTests()
        {
            _userRepoMock = new Mock<IBaseRepository<User>>();
            _newsRepoMock = new Mock<IBaseRepository<News>>();
            _subscriptionRepoMock = new Mock<IBaseRepository<UserSubscription>>();
            _service = new PersonalizedNewsService(_userRepoMock.Object, _newsRepoMock.Object, _subscriptionRepoMock.Object);
        }

        [Fact]
        public async Task GetPersonalizedNewsAsync_ReturnsPagedAndScoredNews()
        {
            
            var userId = 1;
            var newsId = 10;
            var categoryId = 100;
            var keywordId = 200;

            var likedNews = new List<User>
            {
                new User
                {
                    UserID = userId,
                    NewsLikeDislikes = new List<NewsLikeDislike>
                    {
                        new NewsLikeDislike { NewsID = newsId, IsLike = true }
                    }
                }
            };
            _userRepoMock.Setup(r => r.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
                .ReturnsAsync(likedNews);

            var user = new User
            {
                UserID = userId,
                SavedNews = new List<News> { new News { NewsID = newsId } }
            };
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);

            var subscriptions = new List<UserSubscription>
            {
                new UserSubscription { UserID = userId, CategoryID = categoryId, KeywordID = keywordId }
            };
            _subscriptionRepoMock.Setup(r => r.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>()))
                .ReturnsAsync(subscriptions);

            var newsList = new List<News>
            {
                new News
                {
                    NewsID = newsId,
                    PublishedAt = DateTime.UtcNow,
                    Categories = new List<Category> { new Category { CategoryID = categoryId } },
                    Keywords = new List<Keyword> { new Keyword { KeywordID = keywordId } }
                }
            };
            _newsRepoMock.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<News, object>>[]>()))
                .ReturnsAsync(newsList);

            
            var (items, totalCount) = await _service.GetPersonalizedNewsAsync(userId, 1, 10);

            
            Assert.Single(items);
            Assert.Equal(newsId, items.First().NewsID);
            Assert.Equal(1, totalCount);
        }

        [Fact]
        public async Task GetPersonalizedNewsAsync_ReturnsEmpty_WhenNoNews()
        {
            
            _userRepoMock.Setup(r => r.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
                .ReturnsAsync(new List<User>());
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User)null);
            _subscriptionRepoMock.Setup(r => r.FindAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserSubscription, bool>>>()))
                .ReturnsAsync(new List<UserSubscription>());
            _newsRepoMock.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<News, object>>[]>()))
                .ReturnsAsync(new List<News>());

            
            var (items, totalCount) = await _service.GetPersonalizedNewsAsync(1, 1, 10);

            
            Assert.Empty(items);
            Assert.Equal(0, totalCount);
        }
    }
}
