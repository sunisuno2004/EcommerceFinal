using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ECommerceDbContext _context;

        public CartRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(
            int userId,
            int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId);
        }

        public async Task<CartItem> AddToCartAsync(
            CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<CartItem?> UpdateCartItemAsync(
            int userId,
            int productId,
            int quantity)
        {
            var item = await GetCartItemAsync(
                userId,
                productId);

            if (item == null)
            {
                return null;
            }

            item.Quantity = quantity;

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> RemoveFromCartAsync(
            int userId,
            int productId)
        {
            var item = await GetCartItemAsync(
                userId,
                productId);

            if (item == null)
            {
                return false;
            }

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);

            await _context.SaveChangesAsync();
        }
    }
}