using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;

        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock =
                new Mock<IOrderRepository>();

            _cartRepositoryMock =
                new Mock<ICartRepository>();

            _productRepositoryMock =
                new Mock<IProductRepository>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object);
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

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _orderRepositoryMock
                .Setup(r => r.CreateOrderAsync(
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

            Assert.Equal(2, result.OrderItems[0].Quantity);
            Assert.Equal(50000, result.OrderItems[0].UnitPrice);
        }

        [Fact]
        public async Task CreateOrderAsync_EmptyCart_ThrowsException()
        {
            // Arrange
            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(new List<CartItem>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
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
                .Setup(r => r.GetProductByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
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

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.CreateOrderAsync(1));
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ReducesStock()
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

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _orderRepositoryMock
                .Setup(r => r.CreateOrderAsync(
                    It.IsAny<Order>()))
                .ReturnsAsync((Order order) => order);

            // Act
            await _orderService.CreateOrderAsync(1);

            // Assert
            Assert.Equal(7, product.StockQuantity);
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

            _cartRepositoryMock
                .Setup(r => r.GetCartAsync(1))
                .ReturnsAsync(cart);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _orderRepositoryMock
                .Setup(r => r.CreateOrderAsync(
                    It.IsAny<Order>()))
                .ReturnsAsync((Order order) => order);

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
            await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.UpdateOrderStatusAsync(
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
                .Setup(r => r.UpdateOrderStatusAsync(
                    1,
                    "Confirmed"))
                .ReturnsAsync(order);

            // Act
            var result =
                await _orderService.UpdateOrderStatusAsync(
                    1,
                    "Confirmed");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Confirmed", result.Status);
        }
    }
}