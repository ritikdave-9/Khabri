using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;


namespace Service
{
    public class NewsService : INewsService
    {
        private IServiceProvider _serviceProvider;



        public NewsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<(IEnumerable<News> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNo = 1,
            int pageSize = 10,
            string categoryId = "",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (pageNo < 1) pageNo = 1;
            if (pageSize < 1) pageSize = 10;

            var newsCategoryRepo = _serviceProvider.GetRequiredService<IBaseRepository<NewsCategory>>();

            Guid? categoryGuid = null;
            if (Guid.TryParse(categoryId, out var parsedCategoryId))
            {
                categoryGuid = parsedCategoryId;
            }

            Expression<Func<NewsCategory, bool>> predicate = nc =>
                (!categoryGuid.HasValue || nc.CategoryID == categoryGuid.Value) &&
                (!startDate.HasValue || nc.CreatedAt >= startDate.Value) &&
                (!endDate.HasValue || nc.CreatedAt <= endDate.Value);

            var orderBys = new List<Func<IQueryable<NewsCategory>, IOrderedQueryable<NewsCategory>>>
            {
                q => q.OrderByDescending(nc => nc.CreatedAt)
            };

            var (linkItems, totalCount) = await newsCategoryRepo.FindPageAsync(
                predicate,
                pageSize,
                pageNo,
                orderBys,
                nc => nc.News 
            );

            var newsItems = linkItems
                .Select(nc => nc.News)
                .Where(news => news != null)
                .ToList();

            return (newsItems, totalCount);
        }
    }
}
