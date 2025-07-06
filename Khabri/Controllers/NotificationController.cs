using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Data.Entity;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IBaseService<Notification> _notificationBaseservice;
        private readonly INotificationService _service;

        public NotificationController(INotificationService service,IBaseService<Notification> notificationService)
        {
            _service = service;
            _notificationBaseservice = notificationService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddNotification([FromQuery] int userId, [FromQuery] int? newsId)
        {
            await _service.AddNotificationAsync(userId, newsId);
            return Ok(new { Message = "Notification added." });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] int userId)
        {
            var notifications = await _service.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("seen")]
        public async Task<IActionResult> MarkAsSeen([FromQuery] int notificationId)
        {
            await _service.MarkAsSeenAsync(notificationId);
            return Ok(new { Message = "Notification marked as seen." });
        }
        [HttpGet("user/all/{id:int}")]
        public async Task<IActionResult> GetAllNotifications(int id)
        {
            var notifications = await _notificationBaseservice.FindAllAsync(n => n.UserID == id && !n.IsSeen,n=>n.News);


            var response = notifications.Select(n => new NotificationResponseDto
            {
                NotificationId = n.NotificationID,
                NewsId = n.NewsID,
                Title = n.News?.Title,
                PublishedAt = n.News?.PublishedAt
            });
            var unseenNotifications = notifications.Where(n => !n.IsSeen).ToList();
            foreach (var notification in unseenNotifications)
            {
                notification.IsSeen = true;
            }
            if (unseenNotifications.Any())
            {
                await _notificationBaseservice.UpdateAllAsync(unseenNotifications);
            }

            return Ok(response);
        }

        [HttpPost("seen-all/{id:int}")]
        public async Task<IActionResult> MarkAllAsSeen(int id)
        {
            var notifications = await _notificationBaseservice.FindAllAsync(n => n.UserID == id && !n.IsSeen);

            foreach (var notification in notifications)
            {
                notification.IsSeen = true;
                await _notificationBaseservice.UpdateAsync(notification);
            }

            return Ok(new { Message = "All notifications marked as seen." });
        }

    }
}
