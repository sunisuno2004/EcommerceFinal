using ECommerce.Business.Models;

namespace ECommerce.Business.Services
{
    public interface IInventoryService
    {
        Task<Product> GetStockAsync(
            int productId);

        Task<Product> AddStockAsync(
            int productId,
            int quantity);

        Task<Product> RemoveStockAsync(
            int productId,
            int quantity);
    }
}