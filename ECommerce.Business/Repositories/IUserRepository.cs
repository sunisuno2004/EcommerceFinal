using ECommerce.Business.Models;

namespace ECommerce.Business.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User> AddUserAsync(User user);
    }
}