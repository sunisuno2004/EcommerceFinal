using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            return await _cartRepository.GetCartAsync(userId);
        }

        public async Task<CartItem> AddToCartAsync(
            int userId,
            int productId,
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    "Product not found.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException(
                    "Insufficient stock.");
            }

            var existingItem =
                await _cartRepository.GetCartItemAsync(
                    userId,
                    productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;

                return await _cartRepository.UpdateCartItemAsync(
                    userId,
                    productId,
                    existingItem.Quantity)
                    ?? existingItem;
            }

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            return await _cartRepository.AddToCartAsync(cartItem);
        }

        public async Task<CartItem?> UpdateCartItemAsync(
            int userId,
            int productId,
            int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    "Product not found.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException(
                    "Insufficient stock.");
            }

            return await _cartRepository.UpdateCartItemAsync(
                userId,
                productId,
                quantity);
        }

        public async Task<bool> RemoveFromCartAsync(
            int userId,
            int productId)
        {
            return await _cartRepository.RemoveFromCartAsync(
                userId,
                productId);
        }

        public async Task ClearCartAsync(int userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
}