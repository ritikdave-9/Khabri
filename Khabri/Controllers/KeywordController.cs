using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Data.Entity;
using Common.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Common.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KeywordController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public KeywordController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllKeywords(
    [FromQuery] int pageNo = 1,
    [FromQuery] int pageSize = 20)
        {
            if (pageNo < 1) pageNo = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var keywordRepo = _serviceProvider.GetRequiredService<IBaseService<Keyword>>();
                var keywords = await keywordRepo.GetAllAsync();

                var totalCount = keywords.Count();
                var pagedKeywords = keywords
                    .OrderBy(k => k.KeywordID)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .Select(k => new { KeywordID = k.KeywordID, KeywordName = k.KeywordText })
                    .ToList();


                return Ok(new
                {
                    Page = pageNo,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = pagedKeywords
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching keywords. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while fetching keywords." });
            }
        }
        [HttpGet("by-category")]
        public async Task<IActionResult> GetKeywordsByCategory(
    [FromQuery] int categoryId,
    [FromQuery] int pageNo = 1,
    [FromQuery] int pageSize = 20)
        {
            if (pageNo < 1) pageNo = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var keywordRepo = _serviceProvider.GetRequiredService<IBaseService<Keyword>>();

                var keywords = await keywordRepo.FindAllAsync(
                    k => k.News.Any(n => n.Categories.Any(c => c.CategoryID == categoryId))
                );

                var totalCount = keywords.Count();
                var pagedKeywords = keywords
                    .OrderBy(k => k.KeywordID)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .Select(k => new { KeywordID = k.KeywordID, KeywordName = k.KeywordText })
                    .ToList();

                return Ok(new
                {
                    Page = pageNo,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = pagedKeywords
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error fetching keywords by category. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while fetching keywords by category." });
            }
        }
        [HttpPut("toggle-active/{id:int}")]
        public async Task<IActionResult> ToggleKeywordActive(int id)
        {
            try
            {
                var _keywordService = _serviceProvider.GetRequiredService<IBaseService<Keyword>>();

                var keyword = await _keywordService.GetByIdAsync(id);
                if (keyword == null)
                    return NotFound(new ErrorResponseDto { Message = "Keyword not found." });

                keyword.IsActive = !keyword.IsActive;
                var updated = await _keywordService.UpdateAsync(keyword);

                if (updated)
                    return Ok(new { keyword.KeywordID, keyword.KeywordText, keyword.IsActive });

                return StatusCode(500, new ErrorResponseDto { Message = "Failed to update keyword status." });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error toggling keyword status: {ex}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while updating keyword status." });
            }
        }


    }
}
