using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();

            _productService =
                new ProductService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "Test Laptop",
                    Price = 50000,
                    StockQuantity = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Mouse",
                    Description = "Wireless Mouse",
                    Price = 1000,
                    StockQuantity = 20
                }
            };

            _repositoryMock
                .Setup(r => r.GetProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result =
                await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Laptop", result[0].Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 10
            };

            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result =
                await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Laptop", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetProductByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result =
                await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddProductAsync_ReturnsAddedProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Keyboard",
                Price = 2000,
                StockQuantity = 15
            };

            _repositoryMock
                .Setup(r => r.AddProductAsync(product))
                .ReturnsAsync(product);

            // Act
            var result =
                await _productService.AddProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Keyboard", result.Name);
            Assert.Equal(2000, result.Price);
        }

        [Fact]
        public async Task DeleteProductAsync_ProductExists_ReturnsTrue()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.DeleteProductAsync(1))
                .ReturnsAsync(true);

            // Act
            var result =
                await _productService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ProductDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.DeleteProductAsync(999))
                .ReturnsAsync(false);

            // Act
            var result =
                await _productService.DeleteProductAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}