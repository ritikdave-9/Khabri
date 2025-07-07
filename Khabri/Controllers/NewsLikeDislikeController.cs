using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Data.Entity;
using Common.Utils;
using Common.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

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
            try
            {
                await _service.LikeAsync(userId, newsId);
                Logger.LogSuccess($"User {userId} liked news {newsId}.");
                return Ok(new { Message = "News liked." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error liking news: userId={userId}, newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while liking news." });
            }
        }

        [HttpPost("dislike")]
        public async Task<IActionResult> Dislike([FromQuery] int userId, [FromQuery] int newsId)
        {
            try
            {
                await _service.DislikeAsync(userId, newsId);
                Logger.LogSuccess($"User {userId} disliked news {newsId}.");
                return Ok(new ErrorResponseDto{ Message = "News disliked." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error disliking news: userId={userId}, newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while disliking news." });
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveReaction([FromQuery] int userId, [FromQuery] int newsId)
        {
            try
            {
                await _service.RemoveReactionAsync(userId, newsId);
                Logger.LogSuccess($"User {userId} removed reaction from news {newsId}.");
                return Ok(new ErrorResponseDto{ Message = "Reaction removed." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error removing reaction: userId={userId}, newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while removing reaction." });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCounts([FromQuery] int newsId)
        {
            try
            {
                var likes = await _service.GetLikesCountAsync(newsId);
                var dislikes = await _service.GetDislikesCountAsync(newsId);
                Logger.LogInformation($"Fetched like/dislike counts for newsId={newsId}: Likes={likes}, Dislikes={dislikes}");
                return Ok(new { Likes = likes, Dislikes = dislikes });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching like/dislike counts: newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while fetching counts." });
            }
        }

        [HttpGet("user-reaction")]
        public async Task<IActionResult> GetUserReaction([FromQuery] int userId, [FromQuery] int newsId)
        {
            try
            {
                var reaction = await _service.GetUserReactionAsync(userId, newsId);
                string reactionStr = reaction == null ? "None" : (reaction.IsLike ? "Like" : "Dislike");
                Logger.LogInformation($"Fetched user reaction: userId={userId}, newsId={newsId}, reaction={reactionStr}");
                return Ok(new { Reaction = reactionStr });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching user reaction: userId={userId}, newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while fetching user reaction." });
            }
        }
    }
}
