using ECommerce.UserService.Models;

namespace ECommerce.UserService.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(
            string email);

        Task<User?> GetByIdAsync(
            int id);

        Task<User> AddUserAsync(
            User user);
    }
}