using ECommerce.Business.Models;

namespace ECommerce.Business.Services
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartAsync(int userId);

        Task<CartItem> AddToCartAsync(
            int userId,
            int productId,
            int quantity);

        Task<CartItem> UpdateCartItemAsync(
            int userId,
            int productId,
            int quantity);

        Task<bool> RemoveFromCartAsync(
            int userId,
            int productId);

        Task ClearCartAsync(int userId);
    }
}