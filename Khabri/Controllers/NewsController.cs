using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Service;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public NewsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetPagedNews(
            [FromQuery] int pageNo = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string categoryId = "",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var newsService = _serviceProvider.GetRequiredService<INewsService>();

                var (items, totalCount) = await newsService.GetPagedNewsAsync(
                    pageNo,
                    pageSize,
                    categoryId,
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

        //[HttpPost("save-article")]
        //public async Task<IActionResult> SaveArticleAsync([FromQuery] string articleId, [FromQuery] string userId)
        //{
        //    try
        //    {
        //        if (!Guid.TryParse(articleId, out var newsGuid) || !Guid.TryParse(userId, out var userGuid))
        //        {
        //            return BadRequest(new { Message = "Invalid articleId or userId format." });
        //        }

        //        var savedNewsService = _serviceProvider.GetRequiredService<ISavedNewsService>();

        //        var savedNews = new SavedNews
        //        {
        //            NewsId = newsGuid,
        //            UserId = userGuid,
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        var result = await savedNewsService.AddAsync(savedNews);

        //        return Ok(new
        //        {
        //            Message = "Article saved successfully.",
        //            SavedNews = result
        //        });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { Message = "Invalid argument.", Details = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred while saving the article.", Details = ex.Message });
        //    }
        //}
    }
}
