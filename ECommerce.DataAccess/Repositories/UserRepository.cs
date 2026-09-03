using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ECommerceDbContext _context;

        public UserRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}