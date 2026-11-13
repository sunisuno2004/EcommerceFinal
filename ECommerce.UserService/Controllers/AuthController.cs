using ECommerce.UserService.DTOs;
using ECommerce.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto dto)
        {
            var user =
                await _authService.RegisterAsync(
                    dto.FullName,
                    dto.Email,
                    dto.Password,
                    dto.Role);

            return Ok(new
            {
                message = "User registered successfully.",
                userId = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            var token =
                await _authService.LoginAsync(
                    dto.Email,
                    dto.Password);

            return Ok(new
            {
                token
            });
        }
    }
}