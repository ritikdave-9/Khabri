using Data.Entity;

namespace Service.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(int userId, int? newsId);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task MarkAsSeenAsync(int notificationId);
    }
}
