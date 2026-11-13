using ECommerce.ProductService.Exceptions;
using ECommerce.ProductService.Models;
using ECommerce.ProductService.Repositories;

namespace ECommerce.ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository
            _productRepository;

        public ProductService(
            IProductRepository productRepository)
        {
            _productRepository =
                productRepository;
        }

        public async Task<List<Product>>
            GetProductsAsync()
        {
            return await _productRepository
                .GetProductsAsync();
        }

        public async Task<Product>
            GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var product =
                await _productRepository
                    .GetProductByIdAsync(id);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            return product;
        }

        public async Task<Product>
            AddProductAsync(Product product)
        {
            ValidateProduct(product);

            product.StockQuantity = 0;

            return await _productRepository
                .AddProductAsync(product);
        }

        public async Task<Product>
            UpdateProductAsync(
                int id,
                Product product)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            ValidateProduct(product);

            var updatedProduct =
                await _productRepository
                    .UpdateProductAsync(
                        id,
                        product);

            if (updatedProduct is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }

            return updatedProduct;
        }

        public async Task<List<Product>>
            AddProductsAsync(
                List<Product> products)
        {
            if (products is null ||
                products.Count == 0)
            {
                throw new ValidationException(
                    "Product list cannot be empty.");
            }

            foreach (var product in products)
            {
                ValidateProduct(product);

                product.StockQuantity = 0;
            }

            return await _productRepository
                .AddProductsAsync(products);
        }

        public async Task DeleteProductAsync(
            int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var deleted =
                await _productRepository
                    .DeleteProductAsync(id);

            if (!deleted)
            {
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
            }
        }

        private static void ValidateProduct(
            Product product)
        {
            if (product is null)
            {
                throw new ValidationException(
                    "Product is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    product.Name))
            {
                throw new ValidationException(
                    "Product name is required.");
            }

            if (product.Price <= 0)
            {
                throw new ValidationException(
                    "Product price must be greater than zero.");
            }
        }
    }
}