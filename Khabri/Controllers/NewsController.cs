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
                return BadRequest(new { Message = "Invalid parameters.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching news.", Details = ex.Message });
            }
        }
        
    
    
        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedNewsForUser([FromQuery] int userId)
        {
            try
            {
                var userRepo = _serviceProvider.GetRequiredService<IBaseService<User>>();
                var mapper = _serviceProvider.GetRequiredService<AutoMapper.IMapper>();

                var user = await userRepo.FindAsync(u=>u.UserID==userId,u=>u.SavedNews);

                if (user == null)
                    return NotFound(new { Message = "User not found." });

                var savedNews =  user.SavedNews?.ToList() ?? new List<News>();

                var newsDtos = mapper.Map<List<NewsResponseDto>>(savedNews);

                return Ok(new { Items= newsDtos});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching saved news.", Details = ex.Message });
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
                    return NotFound(new { Message = "User not found." });

                var newsToRemove = user.SavedNews.FirstOrDefault(n => n.NewsID == newsId);
                if (newsToRemove == null)
                    return NotFound(new { Message = "News not found in user's saved news." });

                user.SavedNews.Remove(newsToRemove);
                await userRepo.UpdateAsync(user);

                return Ok(new { Message = "News removed from saved news." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting saved news.", Details = ex.Message });
            }
        }
    } 
}
