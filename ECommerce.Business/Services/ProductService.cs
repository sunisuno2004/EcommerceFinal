using ECommerce.Business.Exceptions;
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

        // --------------------------------------------------
        // GET ALL PRODUCTS
        // --------------------------------------------------
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _repository.GetProductsAsync();
        }

        // --------------------------------------------------
        // GET PRODUCT BY ID
        // --------------------------------------------------
        public async Task<Product> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var product =
                await _repository.GetProductByIdAsync(id);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            return product;
        }

        // --------------------------------------------------
        // ADD PRODUCT
        // --------------------------------------------------
        public async Task<Product> AddProductAsync(
            Product product)
        {
            ValidateProduct(product);

            return await _repository.AddProductAsync(product);
        }

        // --------------------------------------------------
        // BULK ADD PRODUCTS
        // --------------------------------------------------
        public async Task<List<Product>> AddProductsAsync(
            List<Product> products)
        {
            if (products is null || products.Count == 0)
            {
                throw new ValidationException(
                    "No products were provided.");
            }

            foreach (var product in products)
            {
                ValidateProduct(product);
            }

            return await _repository.AddProductsAsync(products);
        }

        // --------------------------------------------------
        // UPDATE PRODUCT
        // --------------------------------------------------
        public async Task<Product> UpdateProductAsync(
            int id,
            Product product)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            ValidateProduct(product);

            var existingProduct =
                await _repository.GetProductByIdAsync(id);

            if (existingProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            var updatedProduct =
                await _repository.UpdateProductAsync(
                    id,
                    product);

            if (updatedProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            return updatedProduct;
        }

        // --------------------------------------------------
        // DELETE PRODUCT
        // --------------------------------------------------
        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var existingProduct =
                await _repository.GetProductByIdAsync(id);

            if (existingProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            var deleted =
                await _repository.DeleteProductAsync(id);

            if (!deleted)
            {
                throw new InvalidOperationException(
                    "The product could not be deleted.");
            }

            return true;
        }

        // --------------------------------------------------
        // COMMON PRODUCT VALIDATION
        // --------------------------------------------------
        private static void ValidateProduct(Product product)
        {
            if (product is null)
            {
                throw new ValidationException(
                    "Product information is required.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ValidationException(
                    "Product name is required.");
            }

            if (product.Price <= 0)
            {
                throw new ValidationException(
                    "Product price must be greater than zero.");
            }

            if (product.StockQuantity < 0)
            {
                throw new ValidationException(
                    "Stock quantity cannot be negative.");
            }
        }
    }
}