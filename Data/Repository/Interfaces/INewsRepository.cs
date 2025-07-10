using Data.Entity;

public interface INewsRepository
{
    Task<(IEnumerable<News> Items, int TotalCount)> GetPersonalizedNewsAsync(
        List<int> likedIds,
        List<int> savedIds,
        List<int> subscribedCategoryIds,
        List<int> subscribedKeywordIds,
        DateTime recentThreshold,
        int pageNo,
        int pageSize);
}
