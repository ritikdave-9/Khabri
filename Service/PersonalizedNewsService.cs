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
        private readonly IBaseRepository<News> _newsRepo;
        private readonly IBaseRepository<UserSubscription> _subscriptionRepo;

        public PersonalizedNewsService(
            IBaseRepository<User> userRepo,
            IBaseRepository<News> newsRepo,
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
                .ToHashSet();

            var user = await _userRepo.FindAsync(u => u.UserID == userId, u => u.SavedNews);
            var savedIds = user?.SavedNews.Select(n => n.NewsID).ToHashSet() ?? new HashSet<int>();

            var subscriptions = await _subscriptionRepo.FindAllAsync(s => s.UserID == userId);
            var subscribedCategoryIds = subscriptions.Where(s => s.CategoryID.HasValue).Select(s => s.CategoryID.Value).ToList();
            var subscribedKeywordIds = subscriptions.Where(s => s.KeywordID.HasValue).Select(s => s.KeywordID.Value).ToList();

            var newsQuery = (await _newsRepo.GetAllAsync(n => n.Categories, n => n.Keywords)).AsQueryable();

            var query = newsQuery
                .Select(n => new
                {
                    News = n,
                    Score =
                        (likedIds.Contains(n.NewsID) ? 100 : 0) +
                        (savedIds.Contains(n.NewsID) ? 80 : 0) +
                        (n.Categories.Any(c => subscribedCategoryIds.Contains(c.CategoryID)) ? 50 : 0) +
                        (n.Keywords.Any(k => subscribedKeywordIds.Contains(k.KeywordID)) ? 30 : 0) +
                        (n.PublishedAt > DateTime.UtcNow.AddDays(-2) ? 10 : 0)
                });

            var totalCount = query.Count();
            var pagedNews = query
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.News.PublishedAt)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.News)
                .ToList();

            return (pagedNews, totalCount);
        }


    }
}