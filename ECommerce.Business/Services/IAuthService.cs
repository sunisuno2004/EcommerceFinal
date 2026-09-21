using ECommerce.Business.Models;

namespace ECommerce.Business.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(
            string fullName,
            string email,
            string password);

        Task<User> LoginAsync(string email, string password);
    }
}