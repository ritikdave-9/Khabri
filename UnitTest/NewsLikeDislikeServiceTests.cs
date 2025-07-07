using System.Threading.Tasks;
using Data.Entity;
using Data.Repository.Interfaces;
using Moq;
using Service;
using Xunit;

namespace UnitTest
{
    public class NewsLikeDislikeServiceTests
    {
        private readonly Mock<IBaseRepository<NewsLikeDislike>> _repoMock;
        private readonly NewsLikeDislikeService _service;

        public NewsLikeDislikeServiceTests()
        {
            _repoMock = new Mock<IBaseRepository<NewsLikeDislike>>();
            _service = new NewsLikeDislikeService(_repoMock.Object);
        }

        [Fact]
        public async Task LikeAsync_AddsLike_WhenNoExistingReaction()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync((NewsLikeDislike)null);

            await _service.LikeAsync(1, 2);

            _repoMock.Verify(r => r.AddAsync(It.Is<NewsLikeDislike>(n => n.UserID == 1 && n.NewsID == 2 && n.IsLike)), Times.Once);
        }

        [Fact]
        public async Task LikeAsync_UpdatesReaction_WhenExistingIsDislike()
        {
            var existing = new NewsLikeDislike { UserID = 1, NewsID = 2, IsLike = false };
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync(existing);

            await _service.LikeAsync(1, 2);

            Assert.True(existing.IsLike);
            _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task DislikeAsync_AddsDislike_WhenNoExistingReaction()
        {
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync((NewsLikeDislike)null);

            await _service.DislikeAsync(1, 2);

            _repoMock.Verify(r => r.AddAsync(It.Is<NewsLikeDislike>(n => n.UserID == 1 && n.NewsID == 2 && !n.IsLike)), Times.Once);
        }

        [Fact]
        public async Task DislikeAsync_UpdatesReaction_WhenExistingIsLike()
        {
            var existing = new NewsLikeDislike { UserID = 1, NewsID = 2, IsLike = true };
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync(existing);

            await _service.DislikeAsync(1, 2);

            Assert.False(existing.IsLike);
            _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task RemoveReactionAsync_Deletes_WhenExists()
        {
            var existing = new NewsLikeDislike { NewsLikeDislikeID = 10, UserID = 1, NewsID = 2 };
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync(existing);

            await _service.RemoveReactionAsync(1, 2);

            _repoMock.Verify(r => r.DeleteAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetLikesCountAsync_CallsRepository()
        {
            _repoMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync(5);

            var count = await _service.GetLikesCountAsync(2);

            Assert.Equal(5, count);
        }

        [Fact]
        public async Task GetDislikesCountAsync_CallsRepository()
        {
            _repoMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync(3);

            var count = await _service.GetDislikesCountAsync(2);

            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetUserReactionAsync_CallsRepository()
        {
            var reaction = new NewsLikeDislike { UserID = 1, NewsID = 2, IsLike = true };
            _repoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<NewsLikeDislike, bool>>>()))
                .ReturnsAsync(reaction);

            var result = await _service.GetUserReactionAsync(1, 2);

            Assert.Equal(reaction, result);
        }
    }
}
