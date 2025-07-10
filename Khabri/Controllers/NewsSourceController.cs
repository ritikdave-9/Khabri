using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;
using Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Common.Enums;
using Common.Utils;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(Role.Admin))]
    public class NewsSourceController : ControllerBase
    {
        private readonly IBaseService<NewsSource> _newsSourceService;

        public NewsSourceController(IBaseService<NewsSource> newsSourceService)
        {
            _newsSourceService = newsSourceService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddNewsSource([FromBody] NewsSourceCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var mappingField = new NewsSourceMappingField
                {
                    Title = request.MappingField.Title,
                    Description = request.MappingField.Description,
                    Url = request.MappingField.Url,
                    ImageUrl = request.MappingField.ImageUrl,
                    Content = request.MappingField.Content,
                    Source = request.MappingField.Source,
                    Author = request.MappingField.Author,
                    PublishedAt = request.MappingField.PublishedAt,
                    NewsListKeyString = request.MappingField.NewsListKeyString,
                    CreatedAt = DateTime.UtcNow,
                    Keywords = request.MappingField.Keywords,
                    Category = request.MappingField.Category,
                    Language = request.MappingField.Language
                };

                var token = new NewsSourceToken
                {
                    Token = request.Token?.Token,
                    CreatedAt = DateTime.UtcNow,
                    TokenKeyString = request.Token.TokenKeyString
                };

                var newsSource = new NewsSource
                {
                    Name = request.Name,
                    BaseURL = request.BaseURL,
                    CreatedAt = DateTime.UtcNow,
                    NewsSourceMappingField = mappingField,
                    NewsSourceToken = token
                };

                var result = await _newsSourceService.AddAsync(newsSource);

                return Ok(new ErrorResponseDto{ Message = "NewsSource added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while adding NewsSource." });
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNewsSources()
        {
            try
            {
                var sources = await _newsSourceService.GetAllAsync();

                var result = sources
                    .Select(s => new
                    {
                        s.NewsSourceID,
                        s.Name,
                        s.BaseURL,
                        Status = s.Status.ToString()
                    })
                    .ToList();

                CustomLogger.LogInformation($"Fetched {result.Count} news sources.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error fetching news sources. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while fetching news sources." });
            }
        }
        [HttpPut("edit/{id:int}")]
        public async Task<IActionResult> EditNewsSource(int id, [FromBody] EditNewsSourceRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existing = await _newsSourceService.FindAsync(
                    s => s.NewsSourceID == id,
                    s => s.NewsSourceToken
                );

                if (existing == null)
                {
                    CustomLogger.LogError($"News source not found for edit: id={id}");
                    return NotFound(new ErrorResponseDto { Message = "News source not found." });
                }

                existing.Name = request.Name;
                existing.BaseURL = request.BaseURL;

                if (request.Status != null &&
    Enum.IsDefined(typeof(NewsSourceStatus), request.Status))
                {
                    existing.Status = request.Status;
                }

                if (existing.NewsSourceToken != null && request.Token != null)
                {
                    existing.NewsSourceToken.Token = request.Token;
                }
                else if (request.Token != null)
                {
                    existing.NewsSourceToken = new NewsSourceToken
                    {
                        Token = request.Token,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                var updated = await _newsSourceService.UpdateAsync(existing);

                if (updated)
                {
                    CustomLogger.LogSuccess($"News source updated: id={id}");
                    return Ok(new ErrorResponseDto { Message = "NewsSource updated successfully." });
                }
                else
                {
                    CustomLogger.LogError($"Failed to update news source: id={id}");
                    return StatusCode(500, new ErrorResponseDto { Message = "Failed to update NewsSource." });
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error updating news source: id={id}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while updating NewsSource." });
            }
        }
        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetNewsSourceByIdFromParam(string id)
        {

            try
            {
                var source = await _newsSourceService.FindAsync(
                    s => s.NewsSourceID == Convert.ToInt32( id),
                    s => s.NewsSourceToken
                );

                if (source == null)
                {
                    CustomLogger.LogError($"News source not found: id={id}");
                    return NotFound(new ErrorResponseDto { Message = "News source not found." });
                }

                var result = new
                {
                    source.NewsSourceID,
                    source.Name,
                    source.BaseURL,
                    source.Status,
                    Token = source.NewsSourceToken?.Token
                };

                CustomLogger.LogInformation($"Fetched news source by id={id}.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error fetching news source by id={id}. Details: {ex.Message}");
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred while fetching the news source." });
            }
        }

    }
}
