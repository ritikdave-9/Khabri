using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Microsoft.AspNetCore.Authorization;
using Common.Dtos;
using Data.Repository.Interfaces;
using Common.Utils;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class NewsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public NewsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetPagedNews(
            [FromQuery] int categoryId,
            [FromQuery] int pageNo = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
        )
        {
            try
            {
                var newsService = _serviceProvider.GetRequiredService<INewsService>();

                var (items, totalCount) = await newsService.GetPagedNewsAsync(
                    categoryId,
                    pageNo,
                    pageSize,
                    startDate,
                    endDate
                );

                Logger.LogInformation($"Paged news fetched: categoryId={categoryId}, pageNo={pageNo}, pageSize={pageSize}, totalCount={totalCount}");

                return Ok(new
                {
                    Page = pageNo,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = items
                });
            }
            catch (ArgumentException ex)
            {
                Logger.LogError($"Invalid parameters for GetPagedNews: categoryId={categoryId}, pageNo={pageNo}, pageSize={pageSize}. Details: {ex.Message}");
                return BadRequest(new ErrorResponseDto { Message = "Invalid parameters." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching paged news: categoryId={categoryId}, pageNo={pageNo}, pageSize={pageSize}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while fetching news." });
            }
        }
        [HttpPost("save")]
        public async Task<IActionResult> SaveNewsForUser([FromQuery] int userId, [FromQuery] int newsId)
        {
            try
            {
                var userRepo = _serviceProvider.GetRequiredService<IBaseService<User>>();
                var newsRepo = _serviceProvider.GetRequiredService<IBaseService<News>>();

                var user = await userRepo.FindAsync(u => u.UserID == userId, u => u.SavedNews);
                if (user == null)
                {
                    Logger.LogError($"User not found for SaveNewsForUser: userId={userId}");
                    return NotFound(new ErrorResponseDto { Message = "User not found." });
                }

                var news = await newsRepo.GetByIdAsync(newsId);
                if (news == null)
                {
                    Logger.LogError($"News not found for SaveNewsForUser: newsId={newsId}");
                    return NotFound(new ErrorResponseDto { Message = "News not found." });
                }

                if (user.SavedNews.Any(n => n.NewsID == newsId))
                {
                    Logger.LogInformation($"News already saved: userId={userId}, newsId={newsId}");
                    return Ok(new ErrorResponseDto { Message = "News already saved." });
                }

                user.SavedNews.Add(news);
                await userRepo.UpdateAsync(user);

                Logger.LogSuccess($"News saved for user: userId={userId}, newsId={newsId}");
                return Ok(new ErrorResponseDto { Message = "News saved successfully." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error saving news: userId={userId}, newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while saving news." });
            }
        }


        [HttpGet("saved/all")]
        public async Task<IActionResult> GetSavedNewsForUser([FromQuery] int userId)
        {
            try
            {
                var userRepo = _serviceProvider.GetRequiredService<IBaseService<User>>();
                var mapper = _serviceProvider.GetRequiredService<AutoMapper.IMapper>();

                var user = await userRepo.FindAsync(u => u.UserID == userId, u => u.SavedNews);

                if (user == null)
                {
                    Logger.LogError($"User not found for GetSavedNewsForUser: userId={userId}");
                    return NotFound(new { Message = "User not found." });
                }

                var savedNews = user.SavedNews?.ToList() ?? new List<News>();
                var newsDtos = mapper.Map<List<NewsResponseDto>>(savedNews);

                Logger.LogInformation($"Fetched saved news for userId={userId}, count={newsDtos.Count}");

                return Ok(new { Items = newsDtos });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching saved news for userId={userId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while fetching saved news." });
            }
        }

        [HttpDelete("saved")]
        public async Task<IActionResult> DeleteSavedNewsForUser([FromQuery] int userId, [FromQuery] int newsId)
        {
            try
            {
                var userRepo = _serviceProvider.GetRequiredService<IBaseService<User>>();
                var user = await userRepo.FindAsync(u => u.UserID == userId, u => u.SavedNews);

                if (user == null)
                {
                    Logger.LogError($"User not found for DeleteSavedNewsForUser: userId={userId}");
                    return NotFound(new ErrorResponseDto { Message = "User not found." });
                }

                var newsToRemove = user.SavedNews.FirstOrDefault(n => n.NewsID == newsId);
                if (newsToRemove == null)
                {
                    Logger.LogError($"News not found in user's saved news: userId={userId}, newsId={newsId}");
                    return NotFound(new ErrorResponseDto { Message = "News not found in user's saved news." });
                }

                user.SavedNews.Remove(newsToRemove);
                await userRepo.UpdateAsync(user);

                Logger.LogSuccess($"News removed from saved news: userId={userId}, newsId={newsId}");

                return Ok(new { Message = "News removed from saved news." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting saved news: userId={userId}, newsId={newsId}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while deleting saved news." });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchNews(
            [FromQuery] string term,
            [FromQuery] int pageNo = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                Logger.LogError("Search term is required for SearchNews.");
                return BadRequest(new ErrorResponseDto { Message = "Search term is required." });
            }

            if (pageNo < 1) pageNo = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var newsService = _serviceProvider.GetRequiredService<IBaseService<News>>();
                var mapper = _serviceProvider.GetRequiredService<AutoMapper.IMapper>();

                var (results, totalCount) = await newsService.SearchPageAsync(
                    term,
                    pageNo,
                    pageSize,
                    n => n.Title,
                    n => n.Description,
                    n => n.Content
                );

                var newsDtos = mapper.Map<List<NewsResponseDto>>(results);

                Logger.LogInformation($"SearchNews: term='{term}', pageNo={pageNo}, pageSize={pageSize}, totalCount={totalCount}");

                return Ok(new
                {
                    Page = pageNo,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = newsDtos
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error searching news: term='{term}', pageNo={pageNo}, pageSize={pageSize}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while searching news."});
            }
        }
    }
}
