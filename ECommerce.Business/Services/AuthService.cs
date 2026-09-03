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
                throw new ArgumentException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Password is required.");
            }

            var existingUser =
                await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
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
        public async Task<User?> LoginAsync(
    string email,
    string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Password is required.");
            }

            var user =
                await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            var passwordValid =
                _passwordHasher.VerifyPassword(
                    password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return null;
            }

            return user;
        }
    }
}