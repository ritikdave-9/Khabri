using AutoMapper;
using Common.Dtos;
using Common.Exceptions;
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
        public async Task<IActionResult> ReportNews(
            [FromQuery] string userId,
            [FromQuery] string newsId,
            [FromBody] ReportNewsRequest request)
        {
            try
            {
                var reportService = _serviceProvider.GetRequiredService<IReportService>();
                await reportService.ReportNewsAsync(Convert.ToInt32(userId), Convert.ToInt32(newsId), request.Reason);
                return Ok(new ErrorResponseDto{ Message = "Report submitted." });
            }
            catch (AlreadyExistsException ex)
            {
                return Conflict(new ErrorResponseDto{ Message = ex.Message });
            }
          
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while reporting news." });
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
                return StatusCode(500, new ErrorResponseDto{ Message = "An error occurred while fetching reports."});
            }
        }
    }
}