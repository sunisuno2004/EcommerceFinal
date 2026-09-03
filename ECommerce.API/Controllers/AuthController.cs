using ECommerce.API.DTOs;
using ECommerce.API.Security;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(
     IAuthService authService,
     ILogger<AuthController> logger,
     JwtTokenService jwtTokenService)
        {
            _authService = authService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto dto)
        {
            _logger.LogInformation(
                "Registration attempt for {Email}.",
                dto.Email);

            var user = await _authService.RegisterAsync(
                dto.FullName,
                dto.Email,
                dto.Password);

            _logger.LogInformation(
                "User registered successfully with ID {UserId}.",
                user.Id);

            return Ok(new
            {
                message = "User registered successfully.",
                userId = user.Id,
                fullName = user.FullName,
                email = user.Email
            });
        }

        [HttpPost("login")]
        
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation(
                "Login attempt for {Email}.",
                dto.Email);

            var user = await _authService.LoginAsync(
                dto.Email,
                dto.Password);

            if (user == null)
            {
                _logger.LogWarning(
                    "Failed login attempt for {Email}.",
                    dto.Email);

                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var token =
                _jwtTokenService.GenerateToken(user);

            _logger.LogInformation(
                "User {UserId} logged in successfully.",
                user.Id);

            return Ok(new
            {
                message = "Login successful.",
                token = token,
                userId = user.Id,
                fullName = user.FullName,
                email = user.Email
            });
        }
    }
}