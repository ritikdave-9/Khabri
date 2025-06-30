using System.Linq.Expressions;
using AutoMapper;
using Common.Dtos;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace Service
{
    public class NewsService : INewsService
    {
        public readonly IServiceProvider _serviceProvider;
        public NewsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<(IEnumerable<NewsResponseDto> Items, int TotalCount)> GetPagedNewsAsync(
         int categoryId,
            int pageNo = 1,
         int pageSize = 10,
         DateTime? startDate = null,
        DateTime? endDate = null)
        {
            var NewsRepo = _serviceProvider.GetRequiredService<IBaseRepository<News>>();

            var effectiveEndDate = endDate ?? DateTime.Today.AddDays(1);
            Expression<Func<News, bool>> predicate = n =>
            n.Categories.Any(c => c.CategoryID == categoryId)
            &&
            (startDate == null || n.PublishedAt.Date >= startDate.Value.Date)
            &&
            n.PublishedAt.Date <= effectiveEndDate.Date;
            var (pagedItems, totalCount) = await NewsRepo.FindPageAsync(
                predicate,
                pageSize,
                pageNo,
                new List<Func<IQueryable<News>, IOrderedQueryable<News>>> { q => q.OrderByDescending(n => n.PublishedAt) }
            );
            var mapper = _serviceProvider.GetRequiredService<IMapper>();
            var dtoList = mapper.Map<List<NewsResponseDto>>(pagedItems);

            return (dtoList, totalCount);
    }

        public async Task SaveNewsForUserAsync(int userId, int newsId)
        {
            var userRepo = _serviceProvider.GetRequiredService<IBaseRepository<User>>();
            var newsRepo = _serviceProvider.GetRequiredService<IBaseRepository<News>>();

            var user = await userRepo.GetByIdAsync(userId);
            var news = await newsRepo.GetByIdAsync(newsId);

            if (user == null || news == null)
                throw new InvalidOperationException("User or News not found.");

            if (user.SavedNews.Any(n => n.NewsID == newsId))
                return;

            user.SavedNews.Add(news);

            await userRepo.UpdateAsync(user);
        }
        
    }
}
