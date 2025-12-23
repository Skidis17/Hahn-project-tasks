using Microsoft.AspNetCore.Mvc;
using Project_tasks.DTO;
using Project_tasks.Services;

// This controller handles authentication-related endpoints

namespace Project_tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        // Login endpoint - Returns JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input", errors = ModelState });
                }

                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        // Test endpoint to verify authentication is working
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Auth controller is working!" });
        }
    }
}