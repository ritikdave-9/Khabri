using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Data.Entity;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsSubscribeController : ControllerBase
    {
        private readonly INewsSubscribeService _service;

        public NewsSubscribeController(INewsSubscribeService service)
        {
            _service = service;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromQuery] int userId, [FromQuery] int? categoryId, [FromQuery] int? keywordId)
        {
            try
            {
                await _service.SubscribeAsync(userId, categoryId, keywordId);
                return Ok(new { Message = "Subscribed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while subscribing.", Details = ex.Message });
            }
        }

        [HttpDelete("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromQuery] int userId, [FromQuery] int? categoryId, [FromQuery] int? keywordId)
        {
            try
            {
                await _service.UnsubscribeAsync(userId, categoryId, keywordId);
                return Ok(new { Message = "Unsubscribed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while unsubscribing.", Details = ex.Message });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserSubscriptions([FromQuery] int userId)
        {
            try
            {
                var subscriptions = await _service.GetUserSubscriptionsAsync(userId);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching subscriptions.", Details = ex.Message });
            }
        }
    }
}
