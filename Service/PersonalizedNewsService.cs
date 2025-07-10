using Data.Entity;
using Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PersonalizedNewsService : IPersonalizedNewsService
    {
        private readonly IBaseRepository<User> _userRepo;
        private readonly INewsRepository _newsRepo;
        private readonly IBaseRepository<UserSubscription> _subscriptionRepo;

        public PersonalizedNewsService(
            IBaseRepository<User> userRepo,
            INewsRepository newsRepo,
            IBaseRepository<UserSubscription> subscriptionRepo)
        {
            _userRepo = userRepo;
            _newsRepo = newsRepo;
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<(IEnumerable<News> Items, int TotalCount)> GetPersonalizedNewsAsync(int userId, int pageNo, int pageSize)
        {
            var likedNewsIds = await _userRepo
                .FindAllAsync(u => u.UserID == userId, u => u.NewsLikeDislikes);
            var likedIds = likedNewsIds
                .SelectMany(u => u.NewsLikeDislikes.Where(l => l.IsLike).Select(l => l.NewsID))
                .ToList();

            var user = await _userRepo.FindAsync(u => u.UserID == userId, u => u.SavedNews);
            var savedIds = user?.SavedNews.Select(n => n.NewsID).ToList() ?? new List<int>();

            var subscriptions = await _subscriptionRepo.FindAllAsync(s => s.UserID == userId);
            var subscribedCategoryIds = subscriptions.Where(s => s.CategoryID.HasValue).Select(s => s.CategoryID.Value).ToList();
            var subscribedKeywordIds = subscriptions.Where(s => s.KeywordID.HasValue).Select(s => s.KeywordID.Value).ToList();

            var recentThreshold = DateTime.UtcNow.AddDays(-2);

            return await _newsRepo.GetPersonalizedNewsAsync(
                likedIds,
                savedIds,
                subscribedCategoryIds,
                subscribedKeywordIds,
                recentThreshold,
                pageNo,
                pageSize
            );
        }



    }
}