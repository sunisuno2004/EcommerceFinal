using ECommerce.Business.Exceptions;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductBatchRepository _batchRepository;

        public InventoryService(
            IProductRepository productRepository,
            IProductBatchRepository batchRepository)
        {
            _productRepository = productRepository;
            _batchRepository = batchRepository;
        }

        public async Task<int> GetAvailableStockAsync(
            int productId)
        {
            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var product =
                await _productRepository
                    .GetProductByIdAsync(productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            return await _batchRepository
                .GetTotalStockByProductIdAsync(
                    productId);
        }
    }
}