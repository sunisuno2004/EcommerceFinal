using ECommerce.ProductService.Models;
using ECommerce.ProductService.Data;
namespace ECommerce.ProductService.Repositories
{
    public interface IProductBatchRepository
    {
        Task<List<ProductBatch>> GetAllBatchesAsync();

        Task<ProductBatch?> GetBatchByIdAsync(int id);

        Task<List<ProductBatch>> GetBatchesByProductIdAsync(
            int productId);

        Task<List<ProductBatch>> GetAvailableBatchesForProductAsync(
            int productId);

        Task<ProductBatch> AddBatchAsync(
            ProductBatch batch);

        Task<ProductBatch?> UpdateBatchAsync(
            int id,
            ProductBatch batch);

        Task<int> GetTotalStockByProductIdAsync(
            int productId);

        Task<bool> DeleteBatchAsync(int id);
    }
}