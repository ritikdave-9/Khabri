using Data.Entity;
using Data.Repository.Interfaces;
using Service.Interfaces;

namespace Service
{
    public class NewsLikeDislikeService : INewsLikeDislikeService
    {
        private readonly IBaseRepository<NewsLikeDislike> _likeRepo;

        public NewsLikeDislikeService(IBaseRepository<NewsLikeDislike> likeRepo)
        {
            _likeRepo = likeRepo;
        }

        public async Task LikeAsync(int userId, int newsId)
        {
            var existing = await _likeRepo.FindAsync(x => x.UserID == userId && x.NewsID == newsId);
            if (existing != null)
            {
                if (!existing.IsLike)
                {
                    existing.IsLike = true;
                    await _likeRepo.UpdateAsync(existing);
                }
                return;
            }
            await _likeRepo.AddAsync(new NewsLikeDislike
            {
                UserID = userId,
                NewsID = newsId,
                IsLike = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task DislikeAsync(int userId, int newsId)
        {
            var existing = await _likeRepo.FindAsync(x => x.UserID == userId && x.NewsID == newsId);
            if (existing != null)
            {
                if (existing.IsLike)
                {
                    existing.IsLike = false;
                    await _likeRepo.UpdateAsync(existing);
                }
                return;
            }
            await _likeRepo.AddAsync(new NewsLikeDislike
            {
                UserID = userId,
                NewsID = newsId,
                IsLike = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task RemoveReactionAsync(int userId, int newsId)
        {
            var existing = await _likeRepo.FindAsync(x => x.UserID == userId && x.NewsID == newsId);
            if (existing != null)
            {
                await _likeRepo.DeleteAsync(existing.NewsLikeDislikeID);
            }
        }

        public async Task<int> GetLikesCountAsync(int newsId)
        {
            return await _likeRepo.CountAsync(x => x.NewsID == newsId && x.IsLike);
        }

        public async Task<int> GetDislikesCountAsync(int newsId)
        {
            return await _likeRepo.CountAsync(x => x.NewsID == newsId && !x.IsLike);
        }

        public async Task<NewsLikeDislike> GetUserReactionAsync(int userId, int newsId)
        {
            return await _likeRepo.FindAsync(x => x.UserID == userId && x.NewsID == newsId);
        }
    }
}
