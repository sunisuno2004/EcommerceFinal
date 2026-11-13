using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Services
{
    public interface IProductBatchService
    {
        Task<List<ProductBatch>> GetBatchesAsync();

        Task<ProductBatch> GetBatchByIdAsync(
            int id);

        Task<List<ProductBatch>> GetBatchesByProductIdAsync(
            int productId);

        Task<ProductBatch> AddBatchAsync(
            ProductBatch batch);

        Task<ProductBatch> UpdateBatchAsync(
            int id,
            ProductBatch batch);

        Task DeleteBatchAsync(
            int id);
    }
}