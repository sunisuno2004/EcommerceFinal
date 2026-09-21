using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Security;

namespace ECommerce.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            PasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterAsync(
            string fullName,
            string email,
            string password)
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
                await _userRepository.GetByEmailAsync(email);

            if (existingUser is not null)
            {
                throw new ConflictException(
                    "A user with this email already exists.");
            }

            var passwordHash =
                _passwordHasher.HashPassword(password);

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash
            };

            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User> LoginAsync(
            string email,
            string password)
        {
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

            var user =
                await _userRepository.GetByEmailAsync(email);

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
                _passwordHasher.VerifyPassword(
                    password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            return user;
        }
    }
}