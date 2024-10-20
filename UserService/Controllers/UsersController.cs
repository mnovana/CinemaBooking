using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTO;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
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
    }
}
