using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class ProductBatchServiceTests
    {
        private readonly Mock<IProductBatchRepository>
            _batchRepositoryMock;

        private readonly Mock<IProductRepository>
            _productRepositoryMock;

        private readonly ProductBatchService
            _service;

        public ProductBatchServiceTests()
        {
            _batchRepositoryMock =
                new Mock<IProductBatchRepository>();

            _productRepositoryMock =
                new Mock<IProductRepository>();

            _service =
                new ProductBatchService(
                    _batchRepositoryMock.Object,
                    _productRepositoryMock.Object);
        }

        [Fact]
        public async Task AddBatchAsync_ValidBatch_AddsBatch()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000,
                StockQuantity = 0
            };

            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "B001",
                Quantity = 10,
                IsActive = true
            };

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.AddBatchAsync(
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(batch);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(10);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateStockQuantityAsync(
                        1,
                        10))
                .ReturnsAsync(product);

            var result =
                await _service.AddBatchAsync(batch);

            Assert.NotNull(result);
            Assert.Equal(1, result.ProductId);
            Assert.Equal(10, result.Quantity);
        }

        [Fact]
        public async Task AddBatchAsync_ProductNotFound_ThrowsNotFoundException()
        {
            var batch = new ProductBatch
            {
                ProductId = 99,
                BatchNumber = "B001",
                Quantity = 10,
                IsActive = true
            };

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(99))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.AddBatchAsync(batch));
        }

        [Fact]
        public async Task UpdateBatchAsync_ValidBatch_PreservesProductId()
        {
            var existingBatch = new ProductBatch
            {
                Id = 1,
                ProductId = 5,
                BatchNumber = "B001",
                Quantity = 10,
                IsActive = true
            };

            var updateBatch = new ProductBatch
            {
                BatchNumber = "B001",
                Quantity = 20,
                IsActive = true
            };

            _batchRepositoryMock
                .Setup(r =>
                    r.GetBatchByIdAsync(1))
                .ReturnsAsync(existingBatch);

            _batchRepositoryMock
                .Setup(r =>
                    r.UpdateBatchAsync(
                        1,
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(
                    (int id, ProductBatch batch) =>
                        batch);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(5))
                .ReturnsAsync(
                    new Product
                    {
                        Id = 5,
                        Name = "Phone",
                        Price = 500
                    });

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(5))
                .ReturnsAsync(20);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateStockQuantityAsync(
                        5,
                        20))
                .ReturnsAsync(
                    new Product
                    {
                        Id = 5,
                        Name = "Phone",
                        Price = 500,
                        StockQuantity = 20
                    });

            var result =
                await _service.UpdateBatchAsync(
                    1,
                    updateBatch);

            Assert.Equal(5, result.ProductId);
            Assert.Equal(20, result.Quantity);
        }

        [Fact]
        public async Task DeleteBatchAsync_ValidBatch_DeletesAndSynchronizesStock()
        {
            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "B001",
                Quantity = 10,
                IsActive = true
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000
            };

            _batchRepositoryMock
                .Setup(r =>
                    r.GetBatchByIdAsync(1))
                .ReturnsAsync(batch);

            _batchRepositoryMock
                .Setup(r =>
                    r.DeleteBatchAsync(1))
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(0);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateStockQuantityAsync(
                        1,
                        0))
                .ReturnsAsync(product);

            await _service.DeleteBatchAsync(1);

            _batchRepositoryMock.Verify(
                r => r.DeleteBatchAsync(1),
                Times.Once);

            _productRepositoryMock.Verify(
                r =>
                    r.UpdateStockQuantityAsync(
                        1,
                        0),
                Times.Once);
        }
    }
}