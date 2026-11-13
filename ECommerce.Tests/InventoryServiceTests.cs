using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class InventoryServiceTests
    {
        private readonly Mock<IProductRepository>
            _productRepositoryMock;

        private readonly Mock<IProductBatchRepository>
            _batchRepositoryMock;

        private readonly InventoryService
            _service;

        public InventoryServiceTests()
        {
            _productRepositoryMock =
                new Mock<IProductRepository>();

            _batchRepositoryMock =
                new Mock<IProductBatchRepository>();

            _service =
                new InventoryService(
                    _productRepositoryMock.Object,
                    _batchRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAvailableStockAsync_ValidProduct_ReturnsBatchStock()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000
            };

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(25);

            var result =
                await _service
                    .GetAvailableStockAsync(1);

            Assert.Equal(25, result);
        }

        [Fact]
        public async Task GetAvailableStockAsync_InvalidProductId_ThrowsValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(
                () =>
                    _service
                        .GetAvailableStockAsync(0));
        }

        [Fact]
        public async Task GetAvailableStockAsync_ProductNotFound_ThrowsNotFoundException()
        {
            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(99))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () =>
                    _service
                        .GetAvailableStockAsync(99));
        }
    }
}