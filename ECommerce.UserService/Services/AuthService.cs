using ECommerce.UserService.Exceptions;
using ECommerce.UserService.Models;
using ECommerce.UserService.Repositories;
using ECommerce.UserService.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.UserService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            PasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // --------------------------------------------------
        // REGISTER
        // --------------------------------------------------
        public async Task<User> RegisterAsync(
            string fullName,
            string email,
            string password,
            string role)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ValidationException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException(
                    "Password is required.");
            }

            var existingUser =
                await _userRepository
                    .GetByEmailAsync(email);

            if (existingUser is not null)
            {
                throw new ConflictException(
                    "A user with this email already exists.");
            }

            var user = new User
            {
                FullName = fullName.Trim(),

                Email = email
                    .Trim()
                    .ToLowerInvariant(),

                PasswordHash =
                    _passwordHasher
                        .HashPassword(password),

                Role =
                    string.IsNullOrWhiteSpace(role)
                        ? "Customer"
                        : role,

                CreatedAt =
                    DateTime.UtcNow,

                IsActive = true
            };

            return await _userRepository
                .AddUserAsync(user);
        }

        // --------------------------------------------------
        // LOGIN
        // --------------------------------------------------
        public async Task<string> LoginAsync(
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException(
                    "Email and password are required.");
            }

            var user =
                await _userRepository
                    .GetByEmailAsync(
                        email.Trim().ToLowerInvariant());

            if (user is null)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException(
                    "User account is inactive.");
            }

            var passwordValid =
                _passwordHasher
                    .VerifyPassword(
                        password,
                        user.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            return GenerateJwtToken(user);
        }

        // --------------------------------------------------
        // JWT
        // --------------------------------------------------
        private string GenerateJwtToken(
            User user)
        {
            var jwtKey =
                _configuration["Jwt:Key"];

            var jwtIssuer =
                _configuration["Jwt:Issuer"];

            var jwtAudience =
                _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT signing key is not configured.");
            }

            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Name,
                        user.FullName),

                    new Claim(
                        ClaimTypes.Email,
                        user.Email),

                    new Claim(
                        ClaimTypes.Role,
                        user.Role)
                };

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires:
                        DateTime.UtcNow.AddHours(2),
                    signingCredentials:
                        credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}