using Data.Entity;
using Data.Repository.Interfaces;
using Service.Interfaces;

namespace Service
{
    public class NewsSubscribeService : INewsSubscribeService
    {
        private readonly IBaseRepository<UserSubscription> _subscriptionRepo;

        public NewsSubscribeService(IBaseRepository<UserSubscription> subscriptionRepo)
        {
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task SubscribeAsync(int userId, int? categoryId, int? keywordId)
        {
            if (categoryId == null && keywordId == null)
                throw new ArgumentException("At least one of categoryId or keywordId must be provided.");

            var exists = await _subscriptionRepo.FindAsync(
                s => s.UserID == userId && s.CategoryID == categoryId && s.KeywordID == keywordId
            );
            if (exists != null)
                throw new InvalidOperationException("Already subscribed.");

            var subscription = new UserSubscription
            {
                UserID = userId,
                CategoryID = categoryId,
                KeywordID = keywordId,
                SubscribedAt = DateTime.UtcNow
            };
            await _subscriptionRepo.AddAsync(subscription);
        }

        public async Task UnsubscribeAsync(int userId, int? categoryId, int? keywordId)
        {
            var subscription = await _subscriptionRepo.FindAsync(
                s => s.UserID == userId && s.CategoryID == categoryId && s.KeywordID == keywordId
            );
            if (subscription != null)
            {
                await _subscriptionRepo.DeleteAsync(subscription.UserSubscriptionID);
            }
        }

        public async Task<IEnumerable<UserSubscription>> GetUserSubscriptionsAsync(int userId)
        {
            return await _subscriptionRepo.FindAllAsync(s => s.UserID == userId, s => s.Category, s => s.Keyword);
        }
    }
}
