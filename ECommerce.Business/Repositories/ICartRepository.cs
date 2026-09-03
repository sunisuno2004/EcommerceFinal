using ECommerce.Business.Models;

namespace ECommerce.Business.Repositories
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetCartAsync(int userId);

        Task<CartItem?> GetCartItemAsync(
            int userId,
            int productId);

        Task<CartItem> AddToCartAsync(
            CartItem cartItem);

        Task<CartItem?> UpdateCartItemAsync(
            int userId,
            int productId,
            int quantity);

        Task<bool> RemoveFromCartAsync(
            int userId,
            int productId);

        Task ClearCartAsync(int userId);
    }
}