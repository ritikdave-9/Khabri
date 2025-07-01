using AutoMapper;
using Common.Dtos;
using Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public ReportController(IServiceProvider serviceProvider, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        [HttpPost("news")]
        public async Task<IActionResult> ReportNews([FromQuery] int reporterId, [FromQuery] int newsId, [FromBody] string reason)
        {
            try
            {
                var reportService = _serviceProvider.GetRequiredService<IReportService>();
                await reportService.ReportNewsAsync(reporterId, newsId, reason);
                return Ok(new { Message = "Report submitted." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while reporting news.", Details = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                var reportService = _serviceProvider.GetRequiredService<IReportService>();
                var reports = await reportService.GetAllReportsAsync();
                var reportDtos = _mapper.Map<List<ReportResponseDto>>(reports);
                return Ok(reportDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching reports.", Details = ex.Message });
            }
        }
    }
}