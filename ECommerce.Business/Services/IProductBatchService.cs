using ECommerce.Business.Models;

namespace ECommerce.Business.Services
{
    public interface IProductBatchService
    {
        Task<List<ProductBatch>> GetAllBatchesAsync();

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