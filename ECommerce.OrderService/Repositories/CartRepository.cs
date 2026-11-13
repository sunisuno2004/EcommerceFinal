using ECommerce.OrderService.Data;
using ECommerce.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderService.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly OrderDbContext _context;

        public CartRepository(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartAsync(
            int userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(
            int userId,
            int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(
                    c =>
                        c.UserId == userId &&
                        c.ProductId == productId);
        }

        public async Task<CartItem> AddCartItemAsync(
            CartItem cartItem)
        {
            await _context.CartItems
                .AddAsync(cartItem);

            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<CartItem?> UpdateCartItemAsync(
            int cartItemId,
            int quantity)
        {
            var cartItem =
                await _context.CartItems
                    .FirstOrDefaultAsync(
                        c => c.Id == cartItemId);

            if (cartItem is null)
            {
                return null;
            }

            cartItem.Quantity = quantity;

            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<bool> RemoveCartItemAsync(
            int cartItemId)
        {
            var cartItem =
                await _context.CartItems
                    .FirstOrDefaultAsync(
                        c => c.Id == cartItemId);

            if (cartItem is null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ClearCartAsync(
            int userId)
        {
            var cartItems =
                await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

            if (cartItems.Count == 0)
            {
                return;
            }

            _context.CartItems
                .RemoveRange(cartItems);

            await _context.SaveChangesAsync();
        }
    }
}