namespace Khabri.Controllers
{
    using AutoMapper;
    using Common.Dtos;
    using Common.Exceptions;
    using Data.Entity;
    using Common.Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Common.Dtos;
    using Service.Interfaces;
    using System.Net;

    namespace API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class UserController : ControllerBase
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public UserController(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            [HttpPost("signup")]
            public async Task<IActionResult> SignUp([FromBody] UserSignupDto dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var createdUser = await _userService.SignUpAsync(dto);

                    return Ok(new
                    {
                        createdUser.UserID,
                        createdUser.FirstName,
                        createdUser.Email,
                        Role = createdUser.Role.ToString()
                    });
                }
                catch (InvalidOperationException ex)
                {
                    return Conflict(new { Message = ex.Message });
                }
                catch (RepositoryException ex)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new
                    {
                        Message = "Database error occurred.",
                        Detail = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new
                    {
                        Message = "Unexpected error occurred.",
                        Detail = ex.Message
                    });
                }
            }
        }
    }

}
