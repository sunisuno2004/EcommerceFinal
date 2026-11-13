using ECommerce.OrderService.Clients;
using ECommerce.OrderService.Exceptions;
using ECommerce.OrderService.Models;
using ECommerce.OrderService.Repositories;

namespace ECommerce.OrderService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductServiceClient _productServiceClient;

        public CartService(
            ICartRepository cartRepository,
            IProductServiceClient productServiceClient)
        {
            _cartRepository = cartRepository;
            _productServiceClient = productServiceClient;
        }

        // --------------------------------------------------
        // GET CART
        // --------------------------------------------------
        public async Task<List<CartItem>> GetCartAsync(
            int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            return await _cartRepository
                .GetCartAsync(userId);
        }

        // --------------------------------------------------
        // ADD TO CART
        // --------------------------------------------------
        public async Task<CartItem> AddToCartAsync(
            int userId,
            int productId,
            int quantity)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _productServiceClient
                    .GetProductAsync(productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            var availableStock =
                await _productServiceClient
                    .GetAvailableStockAsync(productId);

            if (availableStock < quantity)
            {
                throw new ConflictException(
                    $"Only {availableStock} item(s) are available.");
            }

            var existingCartItem =
                await _cartRepository
                    .GetCartItemAsync(
                        userId,
                        productId);

            if (existingCartItem is not null)
            {
                var newQuantity =
                    existingCartItem.Quantity +
                    quantity;

                if (availableStock < newQuantity)
                {
                    throw new ConflictException(
                        $"Only {availableStock} item(s) are available.");
                }

                var updatedItem =
                    await _cartRepository
                        .UpdateCartItemAsync(
                            existingCartItem.Id,
                            newQuantity);

                if (updatedItem is null)
                {
                    throw new NotFoundException(
                        "Cart item could not be updated.");
                }

                return updatedItem;
            }

            var cartItem =
                new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

            return await _cartRepository
                .AddCartItemAsync(cartItem);
        }

        // --------------------------------------------------
        // UPDATE CART ITEM BY PRODUCT ID
        // --------------------------------------------------
        public async Task<CartItem> UpdateCartItemAsync(
            int userId,
            int productId,
            int quantity)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }

            var cartItem =
                await _cartRepository
                    .GetCartItemAsync(
                        userId,
                        productId);

            if (cartItem is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found in the cart.");
            }

            var availableStock =
                await _productServiceClient
                    .GetAvailableStockAsync(productId);

            if (availableStock < quantity)
            {
                throw new ConflictException(
                    $"Only {availableStock} item(s) are available.");
            }

            var updatedItem =
                await _cartRepository
                    .UpdateCartItemAsync(
                        cartItem.Id,
                        quantity);

            if (updatedItem is null)
            {
                throw new NotFoundException(
                    "Cart item could not be updated.");
            }

            return updatedItem;
        }

        // --------------------------------------------------
        // REMOVE FROM CART BY PRODUCT ID
        // --------------------------------------------------
        public async Task RemoveFromCartAsync(
            int userId,
            int productId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            if (productId <= 0)
            {
                throw new ValidationException(
                    "Product ID must be greater than zero.");
            }

            var cartItem =
                await _cartRepository
                    .GetCartItemAsync(
                        userId,
                        productId);

            if (cartItem is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found in the cart.");
            }

            var removed =
                await _cartRepository
                    .RemoveCartItemAsync(
                        cartItem.Id);

            if (!removed)
            {
                throw new NotFoundException(
                    "Cart item could not be removed.");
            }
        }

        // --------------------------------------------------
        // CLEAR CART
        // --------------------------------------------------
        public async Task ClearCartAsync(
            int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            await _cartRepository
                .ClearCartAsync(userId);
        }
    }
}