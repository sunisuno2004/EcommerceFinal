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
        private readonly JwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            JwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        // --------------------------------------------------
        // REGISTER
        // POST: /api/Auth/register
        // --------------------------------------------------
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
                "User {UserId} registered successfully.",
                user.Id);

            return Ok(new
            {
                message = "User registered successfully.",
                userId = user.Id,
                email = user.Email,
                role = user.Role
            });
        }

        // --------------------------------------------------
        // LOGIN
        // POST: /api/Auth/login
        // --------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            _logger.LogInformation(
                "Login attempt for {Email}.",
                dto.Email);

            var user = await _authService.LoginAsync(
                dto.Email,
                dto.Password);

            var token =
                _jwtTokenService.GenerateToken(user);

            _logger.LogInformation(
                "User {UserId} logged in successfully.",
                user.Id);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
    }
}