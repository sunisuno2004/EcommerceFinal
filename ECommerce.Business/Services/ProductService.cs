using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _repository.GetProductsAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Product ID must be greater than zero.");
            }

            return await _repository.GetProductByIdAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            // Business validation
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException(
                    "Product name is required.");
            }

            if (product.Price <= 0)
            {
                throw new ArgumentException(
                    "Product price must be greater than zero.");
            }

            if (product.StockQuantity < 0)
            {
                throw new ArgumentException(
                    "Stock quantity cannot be negative.");
            }

            return await _repository.AddProductAsync(product);
        }

        public async Task<Product?> UpdateProductAsync(
            int id,
            Product product)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Product ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException(
                    "Product name is required.");
            }

            if (product.Price <= 0)
            {
                throw new ArgumentException(
                    "Product price must be greater than zero.");
            }

            if (product.StockQuantity < 0)
            {
                throw new ArgumentException(
                    "Stock quantity cannot be negative.");
            }

            return await _repository.UpdateProductAsync(
                id,
                product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Product ID must be greater than zero.");
            }

            return await _repository.DeleteProductAsync(id);
        }
    }
}