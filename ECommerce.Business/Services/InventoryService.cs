using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _repository;

        public InventoryService(
            IProductRepository repository)
        {
            _repository = repository;
        }

        // --------------------------------------------------
        // GET STOCK
        // --------------------------------------------------
        public async Task<Product> GetStockAsync(
            int productId)
        {
            ValidateProductId(productId);

            var product =
                await _repository.GetProductByIdAsync(
                    productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            return product;
        }

        // --------------------------------------------------
        // ADD STOCK
        // --------------------------------------------------
        public async Task<Product> AddStockAsync(
            int productId,
            int quantity)
        {
            ValidateProductId(productId);
            ValidateQuantity(quantity);

            var product =
                await _repository.GetProductByIdAsync(
                    productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            product.StockQuantity += quantity;

            var updatedProduct =
                await _repository.UpdateProductAsync(
                    productId,
                    product);

            if (updatedProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            return updatedProduct;
        }

        // --------------------------------------------------
        // REMOVE STOCK
        // --------------------------------------------------
        public async Task<Product> RemoveStockAsync(
            int productId,
            int quantity)
        {
            ValidateProductId(productId);
            ValidateQuantity(quantity);

            var product =
                await _repository.GetProductByIdAsync(
                    productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new ConflictException(
                    $"Insufficient stock for product " +
                    $"'{product.Name}'. " +
                    $"Available stock: {product.StockQuantity}.");
            }

            product.StockQuantity -= quantity;

            var updatedProduct =
                await _repository.UpdateProductAsync(
                    productId,
                    product);

            if (updatedProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            return updatedProduct;
        }

        // --------------------------------------------------
        // VALIDATE PRODUCT ID
        // --------------------------------------------------
        private static void ValidateProductId(
            int productId)
        {
            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }
        }

        // --------------------------------------------------
        // VALIDATE QUANTITY
        // --------------------------------------------------
        private static void ValidateQuantity(
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }
        }
    }
}