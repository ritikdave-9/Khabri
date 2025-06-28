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

        
    }
}
