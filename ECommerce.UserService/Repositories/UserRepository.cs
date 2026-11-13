using ECommerce.UserService.Data;
using ECommerce.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(
            UserDbContext context)
        {
            _context = context;
        }

        public async Task<User?>
            GetByEmailAsync(
                string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    user =>
                        user.Email == email);
        }

        public async Task<User?>
            GetByIdAsync(
                int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    user =>
                        user.Id == id);
        }

        public async Task<User>
            AddUserAsync(
                User user)
        {
            await _context.Users
                .AddAsync(user);

            await _context
                .SaveChangesAsync();

            return user;
        }
    }
}