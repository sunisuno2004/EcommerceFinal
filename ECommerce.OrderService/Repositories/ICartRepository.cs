using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Repositories
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetCartAsync(
            int userId);

        Task<CartItem?> GetCartItemAsync(
            int userId,
            int productId);

        Task<CartItem> AddCartItemAsync(
            CartItem cartItem);

        Task<CartItem?> UpdateCartItemAsync(
            int cartItemId,
            int quantity);

        Task<bool> RemoveCartItemAsync(
            int cartItemId);

        Task ClearCartAsync(
            int userId);
    }
}