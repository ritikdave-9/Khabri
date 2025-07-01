using Data.Entity;

namespace Service.Interfaces
{
    public interface INewsSubscribeService
    {
        Task SubscribeAsync(int userId, int? categoryId, int? keywordId);
        Task UnsubscribeAsync(int userId, int? categoryId, int? keywordId);
        Task<IEnumerable<UserSubscription>> GetUserSubscriptionsAsync(int userId);
    }
}
