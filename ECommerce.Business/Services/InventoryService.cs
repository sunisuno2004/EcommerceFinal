using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _repository;

        public InventoryService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Product?> GetStockAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException(
                    "Product ID must be greater than zero.");
            }

            return await _repository.GetProductByIdAsync(productId);
        }

        public async Task<Product?> AddStockAsync(
            int productId,
            int quantity)
        {
            if (productId <= 0)
            {
                throw new ArgumentException(
                    "Product ID must be greater than zero.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _repository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return null;
            }

            product.StockQuantity += quantity;

            return await _repository.UpdateProductAsync(
                productId,
                product);
        }

        public async Task<Product?> RemoveStockAsync(
            int productId,
            int quantity)
        {
            if (productId <= 0)
            {
                throw new ArgumentException(
                    "Product ID must be greater than zero.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _repository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return null;
            }

            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException(
                    "Insufficient stock available.");
            }

            product.StockQuantity -= quantity;

            return await _repository.UpdateProductAsync(
                productId,
                product);
        }
    }
}