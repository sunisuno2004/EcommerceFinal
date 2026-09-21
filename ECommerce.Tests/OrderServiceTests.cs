using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository>
            _orderRepositoryMock;

        private readonly Mock<ICartRepository>
            _cartRepositoryMock;

        private readonly Mock<IProductRepository>
            _productRepositoryMock;

        private readonly Mock<IProductBatchRepository>
            _batchRepositoryMock;
        private readonly Mock<IUnitOfWork>
    _unitOfWorkMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock =
                new Mock<IOrderRepository>();

            _cartRepositoryMock =
                new Mock<ICartRepository>();

            _productRepositoryMock =
                new Mock<IProductRepository>();

            _batchRepositoryMock =
                new Mock<IProductBatchRepository>();
            _unitOfWorkMock =
    new Mock<IUnitOfWork>();

            _orderService = new OrderService(
    _orderRepositoryMock.Object,
    _cartRepositoryMock.Object,
    _productRepositoryMock.Object,
    _batchRepositoryMock.Object,
    _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidCart_CreatesOrder()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 10
            };

            var cart = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    UserId = 1,
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 50000
                }
            };

            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "LAP-001",
                Quantity = 10,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetAvailableBatchesForProductAsync(1))
                .ReturnsAsync(new List<ProductBatch>
                {
                    batch
                });

            _batchRepositoryMock
                .Setup(r =>
                    r.UpdateBatchAsync(
                        It.IsAny<int>(),
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(
                    (int id, ProductBatch updatedBatch) =>
                        updatedBatch);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(8);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateProductAsync(
                        1,
                        It.IsAny<Product>()))
                .ReturnsAsync(
                    (int id, Product updatedProduct) =>
                        updatedProduct);

            _orderRepositoryMock
                .Setup(r =>
                    r.CreateOrderAsync(
                        It.IsAny<Order>()))
                .ReturnsAsync((Order order) =>
                {
                    order.Id = 1;
                    return order;
                });

            // Act
            var result =
                await _orderService.CreateOrderAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.UserId);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(100000, result.TotalAmount);
            Assert.Single(result.OrderItems);

            Assert.Equal(
                2,
                result.OrderItems[0].Quantity);

            Assert.Equal(
                50000,
                result.OrderItems[0].UnitPrice);

            Assert.Equal(8, batch.Quantity);
            Assert.Equal(8, product.StockQuantity);
        }

        [Fact]
        public async Task CreateOrderAsync_EmptyCart_ThrowsException()
        {
            // Arrange
            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(new List<CartItem>());

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
                () => _orderService.CreateOrderAsync(1));
        }

        [Fact]
        public async Task CreateOrderAsync_ProductNotFound_ThrowsException()
        {
            // Arrange
            var cart = new List<CartItem>
            {
                new CartItem
                {
                    UserId = 1,
                    ProductId = 999,
                    Quantity = 2
                }
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _orderService.CreateOrderAsync(1));
        }

        [Fact]
        public async Task CreateOrderAsync_InsufficientStock_ThrowsException()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 2
            };

            var cart = new List<CartItem>
            {
                new CartItem
                {
                    UserId = 1,
                    ProductId = 1,
                    Quantity = 5
                }
            };

            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "LAP-001",
                Quantity = 2,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetAvailableBatchesForProductAsync(1))
                .ReturnsAsync(new List<ProductBatch>
                {
                    batch
                });

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
                () => _orderService.CreateOrderAsync(1));
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ReducesBatchStock()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 10
            };

            var cart = new List<CartItem>
            {
                new CartItem
                {
                    UserId = 1,
                    ProductId = 1,
                    Quantity = 3
                }
            };

            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "LAP-001",
                Quantity = 10,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetAvailableBatchesForProductAsync(1))
                .ReturnsAsync(new List<ProductBatch>
                {
                    batch
                });

            _batchRepositoryMock
                .Setup(r =>
                    r.UpdateBatchAsync(
                        It.IsAny<int>(),
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(
                    (int id, ProductBatch updatedBatch) =>
                        updatedBatch);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(7);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateProductAsync(
                        1,
                        It.IsAny<Product>()))
                .ReturnsAsync(
                    (int id, Product updatedProduct) =>
                        updatedProduct);

            _orderRepositoryMock
                .Setup(r =>
                    r.CreateOrderAsync(
                        It.IsAny<Order>()))
                .ReturnsAsync(
                    (Order order) => order);

            // Act
            await _orderService.CreateOrderAsync(1);

            // Assert
            Assert.Equal(7, batch.Quantity);
            Assert.Equal(7, product.StockQuantity);

            _batchRepositoryMock.Verify(
                r => r.UpdateBatchAsync(
                    1,
                    It.Is<ProductBatch>(
                        b => b.Quantity == 7)),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_UsesFefoAcrossBatches()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Milk",
                Price = 50,
                StockQuantity = 15
            };

            var cart = new List<CartItem>
            {
                new CartItem
                {
                    UserId = 1,
                    ProductId = 1,
                    Quantity = 6
                }
            };

            var firstBatch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "MILK-001",
                Quantity = 3,
                ExpiryDate =
                    DateTime.UtcNow.AddDays(5)
            };

            var secondBatch = new ProductBatch
            {
                Id = 2,
                ProductId = 1,
                BatchNumber = "MILK-002",
                Quantity = 5,
                ExpiryDate =
                    DateTime.UtcNow.AddDays(10)
            };

            var thirdBatch = new ProductBatch
            {
                Id = 3,
                ProductId = 1,
                BatchNumber = "MILK-003",
                Quantity = 7,
                ExpiryDate =
                    DateTime.UtcNow.AddDays(20)
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Repository returns batches in FEFO order.
            _batchRepositoryMock
                .Setup(r =>
                    r.GetAvailableBatchesForProductAsync(1))
                .ReturnsAsync(new List<ProductBatch>
                {
                    firstBatch,
                    secondBatch,
                    thirdBatch
                });

            _batchRepositoryMock
                .Setup(r =>
                    r.UpdateBatchAsync(
                        It.IsAny<int>(),
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(
                    (int id, ProductBatch updatedBatch) =>
                        updatedBatch);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(9);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateProductAsync(
                        1,
                        It.IsAny<Product>()))
                .ReturnsAsync(
                    (int id, Product updatedProduct) =>
                        updatedProduct);

            _orderRepositoryMock
                .Setup(r =>
                    r.CreateOrderAsync(
                        It.IsAny<Order>()))
                .ReturnsAsync(
                    (Order order) => order);

            // Act
            await _orderService.CreateOrderAsync(1);

            // Assert
            // 6 ordered:
            // first batch 3 -> 0
            // second batch 5 -> 2
            // third batch stays 7

            Assert.Equal(0, firstBatch.Quantity);
            Assert.Equal(2, secondBatch.Quantity);
            Assert.Equal(7, thirdBatch.Quantity);

            Assert.Equal(9, product.StockQuantity);

            _batchRepositoryMock.Verify(
                r => r.UpdateBatchAsync(
                    1,
                    It.Is<ProductBatch>(
                        b => b.Quantity == 0)),
                Times.Once);

            _batchRepositoryMock.Verify(
                r => r.UpdateBatchAsync(
                    2,
                    It.Is<ProductBatch>(
                        b => b.Quantity == 2)),
                Times.Once);

            _batchRepositoryMock.Verify(
                r => r.UpdateBatchAsync(
                    3,
                    It.IsAny<ProductBatch>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ClearsCart()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Mouse",
                Price = 1000,
                StockQuantity = 20
            };

            var cart = new List<CartItem>
            {
                new CartItem
                {
                    UserId = 1,
                    ProductId = 1,
                    Quantity = 2
                }
            };

            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "MOUSE-001",
                Quantity = 20
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r =>
                    r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetAvailableBatchesForProductAsync(1))
                .ReturnsAsync(new List<ProductBatch>
                {
                    batch
                });

            _batchRepositoryMock
                .Setup(r =>
                    r.UpdateBatchAsync(
                        It.IsAny<int>(),
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(
                    (int id, ProductBatch updatedBatch) =>
                        updatedBatch);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(18);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateProductAsync(
                        1,
                        It.IsAny<Product>()))
                .ReturnsAsync(
                    (int id, Product updatedProduct) =>
                        updatedProduct);

            _orderRepositoryMock
                .Setup(r =>
                    r.CreateOrderAsync(
                        It.IsAny<Order>()))
                .ReturnsAsync(
                    (Order order) => order);

            // Act
            await _orderService.CreateOrderAsync(1);

            // Assert
            _cartRepositoryMock.Verify(
                r => r.ClearCartAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_InvalidStatus_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () =>
                    _orderService.UpdateOrderStatusAsync(
                        1,
                        "InvalidStatus"));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ValidStatus_ReturnsOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                UserId = 1,
                Status = "Confirmed"
            };

            _orderRepositoryMock
                .Setup(r =>
                    r.UpdateOrderStatusAsync(
                        1,
                        "Confirmed"))
                .ReturnsAsync(order);

            // Act
            var result =
                await _orderService
                    .UpdateOrderStatusAsync(
                        1,
                        "Confirmed");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                "Confirmed",
                result.Status);
        }
        [Fact]
        public async Task CreateOrderAsync_Success_CommitsTransaction()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 10
            };

            var cart = new List<CartItem>
    {
        new CartItem
        {
            UserId = 1,
            ProductId = 1,
            Quantity = 2
        }
    };

            var batch = new ProductBatch
            {
                Id = 1,
                ProductId = 1,
                BatchNumber = "LAP-001",
                Quantity = 10
            };

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetAvailableBatchesForProductAsync(1))
                .ReturnsAsync(new List<ProductBatch>
                {
            batch
                });

            _batchRepositoryMock
                .Setup(r =>
                    r.UpdateBatchAsync(
                        It.IsAny<int>(),
                        It.IsAny<ProductBatch>()))
                .ReturnsAsync(
                    (int id, ProductBatch b) => b);

            _batchRepositoryMock
                .Setup(r =>
                    r.GetTotalStockByProductIdAsync(1))
                .ReturnsAsync(8);

            _productRepositoryMock
                .Setup(r =>
                    r.UpdateProductAsync(
                        1,
                        It.IsAny<Product>()))
                .ReturnsAsync(
                    (int id, Product p) => p);

            _orderRepositoryMock
                .Setup(r =>
                    r.CreateOrderAsync(
                        It.IsAny<Order>()))
                .ReturnsAsync(
                    (Order order) => order);

            // Act
            await _orderService.CreateOrderAsync(1);

            // Assert
            _unitOfWorkMock.Verify(
                u => u.BeginTransactionAsync(),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.RollbackTransactionAsync(),
                Times.Never);
        }
        [Fact]
        public async Task CreateOrderAsync_Failure_RollsBackTransaction()
        {
            // Arrange
            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(new List<CartItem>());

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
                () => _orderService.CreateOrderAsync(1));

            _unitOfWorkMock.Verify(
                u => u.BeginTransactionAsync(),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.RollbackTransactionAsync(),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.CommitTransactionAsync(),
                Times.Never);
        }
    }
}