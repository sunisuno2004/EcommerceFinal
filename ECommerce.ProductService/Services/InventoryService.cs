using ECommerce.ProductService.Exceptions;
using ECommerce.ProductService.Repositories;

namespace ECommerce.ProductService.Services
{
    public class InventoryService
        : IInventoryService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductBatchRepository _productBatchRepository;

        public InventoryService(
    IProductRepository productRepository,
    IProductBatchRepository productBatchRepository)
        {
            _productRepository = productRepository;
            _productBatchRepository = productBatchRepository;
        }

        public async Task<int>
            GetAvailableStockAsync(
                int productId)
        {
            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var product =
                await _productRepository
                    .GetProductByIdAsync(
                        productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            return await _productBatchRepository
                .GetTotalStockByProductIdAsync(
                    productId);
        }
        public async Task DeductStockAsync(
    int productId,
    int quantity)
        {
            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _productRepository
                    .GetProductByIdAsync(productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            var batches =
                await _productBatchRepository
                    .GetAvailableBatchesForProductAsync(
                        productId);

            var totalAvailableStock =
                batches.Sum(b => b.Quantity);

            if (totalAvailableStock < quantity)
            {
                throw new ConflictException(
                    $"Insufficient stock for product '{product.Name}'.");
            }

            var remainingQuantity = quantity;

            foreach (var batch in batches)
            {
                if (remainingQuantity == 0)
                {
                    break;
                }

                var quantityToDeduct =
                    Math.Min(
                        batch.Quantity,
                        remainingQuantity);

                batch.Quantity -= quantityToDeduct;

                remainingQuantity -= quantityToDeduct;

                await _productBatchRepository
                    .UpdateBatchAsync(
                        batch.Id,
                        batch);
            }

            var remainingStock =
                await _productBatchRepository
                    .GetTotalStockByProductIdAsync(
                        productId);

            await _productRepository
                .UpdateStockQuantityAsync(
                    productId,
                    remainingStock);
        }
    }
}