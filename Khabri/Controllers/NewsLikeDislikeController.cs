using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Data.Entity;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsLikeDislikeController : ControllerBase
    {
        private readonly INewsLikeDislikeService _service;

        public NewsLikeDislikeController(INewsLikeDislikeService service)
        {
            _service = service;
        }

        [HttpPost("like")]
        public async Task<IActionResult> Like([FromQuery] int userId, [FromQuery] int newsId)
        {
            await _service.LikeAsync(userId, newsId);
            return Ok(new { Message = "News liked." });
        }

        [HttpPost("dislike")]
        public async Task<IActionResult> Dislike([FromQuery] int userId, [FromQuery] int newsId)
        {
            await _service.DislikeAsync(userId, newsId);
            return Ok(new { Message = "News disliked." });
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveReaction([FromQuery] int userId, [FromQuery] int newsId)
        {
            await _service.RemoveReactionAsync(userId, newsId);
            return Ok(new { Message = "Reaction removed." });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCounts([FromQuery] int newsId)
        {
            var likes = await _service.GetLikesCountAsync(newsId);
            var dislikes = await _service.GetDislikesCountAsync(newsId);
            return Ok(new { Likes = likes, Dislikes = dislikes });
        }

        [HttpGet("user-reaction")]
        public async Task<IActionResult> GetUserReaction([FromQuery] int userId, [FromQuery] int newsId)
        {
            var reaction = await _service.GetUserReactionAsync(userId, newsId);
            if (reaction == null)
                return Ok(new { Reaction = "None" });
            return Ok(new { Reaction = reaction.IsLike ? "Like" : "Dislike" });
        }
    }
}
