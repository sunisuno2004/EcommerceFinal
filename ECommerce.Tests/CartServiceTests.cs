using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.Business.Services;
using Moq;

namespace ECommerce.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _cartService = new CartService(
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object);
        }

        [Fact]
        public async Task AddToCartAsync_ProductExists_AddsProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 10
            };

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepositoryMock
                .Setup(r => r.GetCartItemAsync(1, 1))
                .ReturnsAsync((CartItem?)null);

            var cartItem = new CartItem
            {
                Id = 1,
                UserId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 50000
            };

            _cartRepositoryMock
                .Setup(r => r.AddToCartAsync(It.IsAny<CartItem>()))
                .ReturnsAsync(cartItem);

            // Act
            var result =
                await _cartService.AddToCartAsync(1, 1, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProductId);
            Assert.Equal(2, result.Quantity);
            Assert.Equal(50000, result.UnitPrice);
        }

        [Fact]
        public async Task AddToCartAsync_InvalidQuantity_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _cartService.AddToCartAsync(1, 1, 0));
        }

        [Fact]
        public async Task AddToCartAsync_ProductDoesNotExist_ThrowsException()
        {
            // Arrange
            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _cartService.AddToCartAsync(1, 999, 2));
        }

        [Fact]
        public async Task AddToCartAsync_InsufficientStock_ThrowsException()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 2
            };

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _cartService.AddToCartAsync(1, 1, 5));
        }

        [Fact]
        public async Task GetCartAsync_ReturnsCartItems()
        {
            // Arrange
            var cartItems = new List<CartItem>
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
                .ReturnsAsync(cartItems);

            // Act
            var result =
                await _cartService.GetCartAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result[0].ProductId);
        }

        [Fact]
        public async Task UpdateCartItemAsync_ValidQuantity_UpdatesItem()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50000,
                StockQuantity = 10
            };

            var cartItem = new CartItem
            {
                Id = 1,
                UserId = 1,
                ProductId = 1,
                Quantity = 3,
                UnitPrice = 50000
            };

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepositoryMock
                .Setup(r => r.UpdateCartItemAsync(1, 1, 3))
                .ReturnsAsync(cartItem);

            // Act
            var result =
                await _cartService.UpdateCartItemAsync(1, 1, 3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Quantity);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ItemExists_ReturnsTrue()
        {
            // Arrange
            _cartRepositoryMock
                .Setup(r => r.RemoveFromCartAsync(1, 1))
                .ReturnsAsync(true);

            // Act
            var result =
                await _cartService.RemoveFromCartAsync(1, 1);

            // Assert
            Assert.True(result);
        }
    }
}