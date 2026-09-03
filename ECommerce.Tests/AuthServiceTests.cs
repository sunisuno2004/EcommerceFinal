using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Security;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly PasswordHasher _passwordHasher;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            _passwordHasher = new PasswordHasher();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasher);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FullName = "John",
                Email = "john@gmail.com",
                Role = "Customer",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync((User?)null);

            _userRepositoryMock
                .Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User newUser) =>
                {
                    newUser.Id = 1;
                    newUser.IsActive = true;
                    return newUser;
                });

            // Act
            var result = await _authService.RegisterAsync(
                "John",
                "john@gmail.com",
                "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FullName);
            Assert.Equal("john@gmail.com", result.Email);
            Assert.Equal("Customer", result.Role);
            Assert.NotEmpty(result.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ThrowsException()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                FullName = "Existing User",
                Email = "john@gmail.com",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(
                    "John",
                    "john@gmail.com",
                    "password123"));
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var passwordHash =
                _passwordHasher.HashPassword("password123");

            var user = new User
            {
                Id = 1,
                FullName = "John",
                Email = "john@gmail.com",
                PasswordHash = passwordHash,
                Role = "Customer",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(
                "john@gmail.com",
                "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("john@gmail.com", result.Email);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var passwordHash =
                _passwordHasher.HashPassword("password123");

            var user = new User
            {
                Id = 1,
                FullName = "John",
                Email = "john@gmail.com",
                PasswordHash = passwordHash,
                Role = "Customer",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(
                "john@gmail.com",
                "wrong-password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync("unknown@gmail.com"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(
                "unknown@gmail.com",
                "password123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FullName = "John",
                Email = "john@gmail.com",
                PasswordHash = "some-hash",
                Role = "Customer",
                IsActive = false
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(
                "john@gmail.com",
                "password123");

            // Assert
            Assert.Null(result);
        }
    }
}