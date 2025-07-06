using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Common.Dtos;
using Service.Interfaces;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Common.Exceptions;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _loginService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService loginService, ILogger<AuthController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _loginService.ValidateUserAsync(loginDto.Email, loginDto.Password);
                return Ok(result);
            }
            catch (AuthException ex)
            {
                _logger.LogWarning(ex, "Authentication failed.");
                return Unauthorized(new ErrorResponseDto { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new ErrorResponseDto { Message = "An internal server error occurred. Please try again later." });
            }
        }

    }
}
