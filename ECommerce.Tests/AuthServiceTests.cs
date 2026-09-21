using ECommerce.Business.Exceptions;
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
            _userRepositoryMock =
                new Mock<IUserRepository>();

            _passwordHasher =
                new PasswordHasher();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasher);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsUser()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r =>
                    r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync((User?)null);

            _userRepositoryMock
                .Setup(r =>
                    r.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User newUser) =>
                {
                    newUser.Id = 1;
                    newUser.IsActive = true;
                    return newUser;
                });

            // Act
            var result =
                await _authService.RegisterAsync(
                    "John",
                    "john@gmail.com",
                    "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FullName);
            Assert.Equal(
                "john@gmail.com",
                result.Email);
            Assert.Equal(
                "Customer",
                result.Role);
            Assert.NotEmpty(result.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ThrowsConflictException()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                FullName = "Existing User",
                Email = "john@gmail.com",
                PasswordHash = "hashed-password",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(r =>
                    r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
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
                _passwordHasher.HashPassword(
                    "password123");

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
                .Setup(r =>
                    r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(user);

            // Act
            var result =
                await _authService.LoginAsync(
                    "john@gmail.com",
                    "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(
                "john@gmail.com",
                result.Email);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var passwordHash =
                _passwordHasher.HashPassword(
                    "password123");

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
                .Setup(r =>
                    r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(
                    "john@gmail.com",
                    "wrong-password"));
        }

        [Fact]
        public async Task LoginAsync_UserDoesNotExist_ThrowsUnauthorizedException()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r =>
                    r.GetByEmailAsync(
                        "unknown@gmail.com"))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(
                    "unknown@gmail.com",
                    "password123"));
        }

        [Fact]
        public async Task LoginAsync_InactiveUser_ThrowsUnauthorizedException()
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
                .Setup(r =>
                    r.GetByEmailAsync("john@gmail.com"))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(
                    "john@gmail.com",
                    "password123"));
        }
    }
}