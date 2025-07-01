using Data.Entity;
using Data.Repository.Interfaces;
using Service.Interfaces;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly IBaseRepository<Notification> _notificationRepo;

        public NotificationService(IBaseRepository<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task AddNotificationAsync(int userId, int? newsId)
        {
            var notification = new Notification
            {
                UserID = userId,
                NewsID = newsId,
                CreatedAt = DateTime.UtcNow,
                IsSeen = false
            };
            await _notificationRepo.AddAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepo.FindAllAsync(n => n.UserID == userId, n => n.News);
        }

        public async Task MarkAsSeenAsync(int notificationId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification != null && !notification.IsSeen)
            {
                notification.IsSeen = true;
                await _notificationRepo.UpdateAsync(notification);
            }
        }
    }
}
