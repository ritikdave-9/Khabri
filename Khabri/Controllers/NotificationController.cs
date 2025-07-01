using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Data.Entity;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
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
    }
}
