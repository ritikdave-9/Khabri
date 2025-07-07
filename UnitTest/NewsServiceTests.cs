using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Common.Dtos;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Service;
using Xunit;

namespace UnitTest
{
    public class NewsServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBaseRepository<News>> _newsRepoMock;
        private readonly NewsService _service;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IBaseRepository<User>> _userRepoMock;
        public NewsServiceTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _newsRepoMock = new Mock<IBaseRepository<News>>();
            _userRepoMock = new Mock<IBaseRepository<User>>();
            _mapperMock = new Mock<IMapper>();

        
            _serviceProviderMock.Setup(x => x.GetService(typeof(IBaseRepository<News>)))
                .Returns(_newsRepoMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IBaseRepository<User>)))
                .Returns(_userRepoMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IMapper)))
                .Returns(_mapperMock.Object);

            _service = new NewsService(_serviceProviderMock.Object);
        }

        [Fact]
        public async Task GetPagedNewsAsync_ReturnsMappedDtosAndTotalCount()
        {
            var newsList = new List<News>
            {
                new News { NewsID = 1, Title = "Test", PublishedAt = DateTime.Today, Categories = new List<Category> { new Category { CategoryID = 1 } } }
            };
            var dtoList = new List<NewsResponseDto>
            {
                new NewsResponseDto { NewsID = 1, Title = "Test" }
            };

            _newsRepoMock.Setup(r => r.FindPageAsync(
                It.IsAny<Expression<Func<News, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<List<Func<IQueryable<News>, IOrderedQueryable<News>>>>()))
                .ReturnsAsync((newsList, 1));

            _mapperMock.Setup(m => m.Map<List<NewsResponseDto>>(newsList)).Returns(dtoList);

            var (items, totalCount) = await _service.GetPagedNewsAsync(1);

            Assert.Single(items);
            Assert.Equal(1, totalCount);
            Assert.Equal("Test", items.First().Title);
        }

        [Fact]
        public async Task SaveNewsForUserAsync_AddsNews_WhenNotAlreadySaved()
        {
            
            var user = new User { UserID = 1, SavedNews = new List<News>() };
            var news = new News { NewsID = 2 };

            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _newsRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(news);
            _userRepoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

            
            await _service.SaveNewsForUserAsync(1, 2);

            
            Assert.Contains(news, user.SavedNews);
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task SaveNewsForUserAsync_DoesNothing_WhenAlreadySaved()
        {
            
            var news = new News { NewsID = 2 };
            var user = new User { UserID = 1, SavedNews = new List<News> { news } };

            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _newsRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(news);

            
            await _service.SaveNewsForUserAsync(1, 2);

            
            _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task SaveNewsForUserAsync_Throws_WhenUserOrNewsNotFound()
        {
            
            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User)null);

            
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SaveNewsForUserAsync(1, 2));
        }
    }
}
