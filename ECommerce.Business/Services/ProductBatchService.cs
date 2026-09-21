using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class ProductBatchService : IProductBatchService
    {
        private readonly IProductBatchRepository _batchRepository;
        private readonly IProductRepository _productRepository;

        public ProductBatchService(
            IProductBatchRepository batchRepository,
            IProductRepository productRepository)
        {
            _batchRepository = batchRepository;
            _productRepository = productRepository;
        }

        // --------------------------------------------------
        // GET ALL BATCHES
        // --------------------------------------------------
        public async Task<List<ProductBatch>> GetAllBatchesAsync()
        {
            return await _batchRepository.GetAllBatchesAsync();
        }

        // --------------------------------------------------
        // GET BATCH BY ID
        // --------------------------------------------------
        public async Task<ProductBatch> GetBatchByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Batch ID must be greater than zero.");
            }

            var batch =
                await _batchRepository.GetBatchByIdAsync(id);

            if (batch is null)
            {
                throw new NotFoundException(
                    $"Batch with ID {id} was not found.");
            }

            return batch;
        }

        // --------------------------------------------------
        // GET BATCHES BY PRODUCT ID
        // --------------------------------------------------
        public async Task<List<ProductBatch>> GetBatchesByProductIdAsync(
            int productId)
        {
            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var product =
                await _productRepository.GetProductByIdAsync(
                    productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            return await _batchRepository
                .GetBatchesByProductIdAsync(productId);
        }

        // --------------------------------------------------
        // ADD BATCH
        // --------------------------------------------------
        public async Task<ProductBatch> AddBatchAsync(
            ProductBatch batch)
        {
            ValidateBatch(batch);

            var product =
                await _productRepository.GetProductByIdAsync(
                    batch.ProductId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {batch.ProductId} " +
                    "was not found.");
            }

            var createdBatch =
                await _batchRepository.AddBatchAsync(batch);

            await SynchronizeProductStockAsync(
                batch.ProductId);

            return createdBatch;
        }

        // --------------------------------------------------
        // UPDATE BATCH
        // --------------------------------------------------
        public async Task<ProductBatch> UpdateBatchAsync(
            int id,
            ProductBatch batch)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Batch ID must be greater than zero.");
            }

            ValidateBatch(batch);

            var existingBatch =
                await _batchRepository.GetBatchByIdAsync(id);

            if (existingBatch is null)
            {
                throw new NotFoundException(
                    $"Batch with ID {id} was not found.");
            }

            // Prevent changing the product through batch update.
            batch.ProductId = existingBatch.ProductId;

            var updatedBatch =
                await _batchRepository.UpdateBatchAsync(
                    id,
                    batch);

            if (updatedBatch is null)
            {
                throw new NotFoundException(
                    $"Batch with ID {id} was not found.");
            }

            await SynchronizeProductStockAsync(
                existingBatch.ProductId);

            return updatedBatch;
        }

        // --------------------------------------------------
        // DELETE BATCH
        // --------------------------------------------------
        public async Task<bool> DeleteBatchAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Batch ID must be greater than zero.");
            }

            var batch =
                await _batchRepository.GetBatchByIdAsync(id);

            if (batch is null)
            {
                throw new NotFoundException(
                    $"Batch with ID {id} was not found.");
            }

            var productId =
                batch.ProductId;

            var deleted =
                await _batchRepository.DeleteBatchAsync(id);

            if (!deleted)
            {
                throw new InvalidOperationException(
                    "The product batch could not be deleted.");
            }

            await SynchronizeProductStockAsync(
                productId);

            return true;
        }

        // --------------------------------------------------
        // SYNCHRONIZE PRODUCT STOCK FROM BATCHES
        // --------------------------------------------------
        private async Task SynchronizeProductStockAsync(
            int productId)
        {
            var product =
                await _productRepository.GetProductByIdAsync(
                    productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            var totalStock =
                await _batchRepository
                    .GetTotalStockByProductIdAsync(
                        productId);

            product.StockQuantity =
                totalStock;

            var updatedProduct =
                await _productRepository.UpdateProductAsync(
                    productId,
                    product);

            if (updatedProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }
        }

        // --------------------------------------------------
        // COMMON BATCH VALIDATION
        // --------------------------------------------------
        private static void ValidateBatch(
            ProductBatch batch)
        {
            if (batch is null)
            {
                throw new ValidationException(
                    "Batch information is required.");
            }

            if (batch.ProductId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(
                    batch.BatchNumber))
            {
                throw new ValidationException(
                    "Batch number is required.");
            }

            if (batch.Quantity < 0)
            {
                throw new ValidationException(
                    "Batch quantity cannot be negative.");
            }

            if (batch.ManufacturingDate.HasValue &&
                batch.ExpiryDate.HasValue &&
                batch.ExpiryDate.Value <
                batch.ManufacturingDate.Value)
            {
                throw new ValidationException(
                    "Expiry date cannot be before " +
                    "manufacturing date.");
            }
        }
    }
}