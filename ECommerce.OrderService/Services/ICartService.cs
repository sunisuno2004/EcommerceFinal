using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Services
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartAsync(
            int userId);

        Task<CartItem> AddToCartAsync(
            int userId,
            int productId,
            int quantity);

        Task<CartItem> UpdateCartItemAsync(
            int userId,
            int cartItemId,
            int quantity);

        Task RemoveFromCartAsync(
            int userId,
            int cartItemId);

        Task ClearCartAsync(
            int userId);
    }
}