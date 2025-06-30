using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Entity;
using Service.Interfaces;
using Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Common.Enums;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = nameof(Role.Admin))]
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

                return Ok(new { Message = "NewsSource added successfully.", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding NewsSource.", Details = ex.Message });
            }
        }
    }
}
