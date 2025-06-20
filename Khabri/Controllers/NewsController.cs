using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IBaseService<News> _newsService;

        public NewsController(IBaseService<News> newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetPagedNews([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10, [FromQuery] string categoryId = "")
        {
            try
            {
                Expression<Func<News, bool>> filter = n => true;

                var orderBys = new List<Func<IQueryable<News>, IOrderedQueryable<News>>>
                {
                    q => q.OrderByDescending(n => n.PublishedAt)
                };

                var (items, totalCount) = await _newsService.FindPageAsync(
                    predicate: filter,
                    pageSize: pageSize,
                    pageNo: pageNo,
                    orderBys: orderBys
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
                return BadRequest(new { Message = "Invalid parameters", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the news.", Details = ex.Message });
            }
        }


    }

   
}
