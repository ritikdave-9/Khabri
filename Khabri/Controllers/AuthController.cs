using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Common.Dtos;
using Service.Interfaces;

namespace Khabri.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                var isValid = await _loginService.ValidateUserAsync(loginDto.Email, loginDto.Password);

                if (!isValid)
                    return Unauthorized(new { message = "Invalid email or password." });

                return Ok(new { message = "Login successful." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
            }
        }
    }
}
