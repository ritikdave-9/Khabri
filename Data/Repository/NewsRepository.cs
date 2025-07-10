using Data.Entity;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Repository;
using Common.Exceptions;

public class NewsRepository : BaseRepository<News>, INewsRepository
{
    public NewsRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<News> Items, int TotalCount)> GetPersonalizedNewsAsync(
        List<int> likedIds,
        List<int> savedIds,
        List<int> subscribedCategoryIds,
        List<int> subscribedKeywordIds,
        DateTime recentThreshold,
        int pageNo,
        int pageSize)
    {
        try
        {
            var query = _context.News
                .Include(n => n.Categories)
                .Include(n => n.Keywords)
                .Select(n => new
                {
                    News = n,
                    Score =
                        (likedIds.Contains(n.NewsID) ? 100 : 0) +
                        (savedIds.Contains(n.NewsID) ? 80 : 0) +
                        (n.Categories.Any(c => subscribedCategoryIds.Contains(c.CategoryID)) ? 50 : 0) +
                        (n.Keywords.Any(k => subscribedKeywordIds.Contains(k.KeywordID)) ? 30 : 0) +
                        (n.PublishedAt > recentThreshold ? 10 : 0)
                });

            var totalCount = await query.CountAsync();

            var pagedNews = await query
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.News.PublishedAt)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.News)
                .ToListAsync();

            return (pagedNews, totalCount);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to get personalized news.", ex);
        }
    }
}