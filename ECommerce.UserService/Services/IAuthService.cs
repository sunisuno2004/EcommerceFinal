using ECommerce.UserService.Models;

namespace ECommerce.UserService.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(
            string fullName,
            string email,
            string password,
            string role);

        Task<string> LoginAsync(
            string email,
            string password);
    }
}