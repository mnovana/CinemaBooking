using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTO;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthService authService, ILogger<UsersController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            var result = await _authService.RegisterAsync(model);
           
            if (!result.Succeeded)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Failed registration",
                    Detail = string.Join(" ", result.Errors.Select(e => e.Description))
                });
            }

            return Ok(model);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var token = await _authService.LoginAsync(model);

            if (token == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Failed login",
                    Detail = "Login failed due to a wrong username or password."
                });
            }

            return Ok(token);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> EmailExists([FromQuery] string email)
        {
            var exists = await _authService.EmailExistsAsync(email);

            if (!exists)
            {
                _logger.LogWarning("User email '{email}' not found.", email);
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "User email not found",
                    Detail = $"The user email '{email}' was not found in the database."
                });
            }

            return Ok();
        }
    }
}
