using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductBatchRepository _batchRepository;

        public CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IProductBatchRepository batchRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _batchRepository = batchRepository;
        }

        // --------------------------------------------------
        // GET CART
        // --------------------------------------------------
        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            // Empty cart is valid.
            // Repository should return an empty list.
            return await _cartRepository.GetCartAsync(userId);
        }

        // --------------------------------------------------
        // ADD TO CART
        // --------------------------------------------------
        public async Task<CartItem> AddToCartAsync(
            int userId,
            int productId,
            int quantity)
        {
            ValidateIds(userId, productId);

            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _productRepository.GetProductByIdAsync(
                    productId);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }
            var availableStock =
    await _batchRepository
        .GetTotalStockByProductIdAsync(
            productId);
            var existingItem =
                await _cartRepository.GetCartItemAsync(
                    userId,
                    productId);

            // --------------------------------------------------
            // PRODUCT ALREADY EXISTS IN CART
            // --------------------------------------------------
            if (existingItem is not null)
            {
                var newQuantity =
                    existingItem.Quantity + quantity;

                if (newQuantity > availableStock)
                {
                    throw new ConflictException(
                        $"Only {availableStock} item(s) " +
                        $"are available for product '{product.Name}'.");
                }

                var updatedItem =
                    await _cartRepository.UpdateCartItemAsync(
                        userId,
                        productId,
                        newQuantity);

                if (updatedItem is null)
                {
                    throw new NotFoundException(
                        "Cart item could not be found after update.");
                }

                return updatedItem;
            }

            // --------------------------------------------------
            // NEW CART ITEM
            // --------------------------------------------------
            if (quantity > availableStock)
            {
                throw new ConflictException(
                    $"Only {availableStock} item(s) " +
                    $"are available for product '{product.Name}'.");
            }
            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            return await _cartRepository.AddToCartAsync(
                cartItem);
        }

        // --------------------------------------------------
        // UPDATE CART ITEM
        // --------------------------------------------------
        public async Task<CartItem> UpdateCartItemAsync(
            int userId,
            int productId,
            int quantity)
        {
            ValidateIds(userId, productId);

            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Quantity must be greater than zero.");
            }

            var product =
                await _productRepository.GetProductByIdAsync(
                    productId);
            var availableStock =
    await _batchRepository
        .GetTotalStockByProductIdAsync(
            productId);
            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} was not found.");
            }

            var existingItem =
                await _cartRepository.GetCartItemAsync(
                    userId,
                    productId);

            if (existingItem is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} " +
                    "was not found in the cart.");
            }

            if (quantity > availableStock)
            {
                throw new ConflictException(
    $"Only {availableStock} item(s) " +
    $"are available for product '{product.Name}'.");
            }

            var updatedItem =
                await _cartRepository.UpdateCartItemAsync(
                    userId,
                    productId,
                    quantity);

            if (updatedItem is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} " +
                    "was not found in the cart.");
            }

            return updatedItem;
        }

        // --------------------------------------------------
        // REMOVE FROM CART
        // --------------------------------------------------
        public async Task<bool> RemoveFromCartAsync(
            int userId,
            int productId)
        {
            ValidateIds(userId, productId);

            var existingItem =
                await _cartRepository.GetCartItemAsync(
                    userId,
                    productId);

            if (existingItem is null)
            {
                throw new NotFoundException(
                    $"Product with ID {productId} " +
                    "was not found in the cart.");
            }

            var removed =
                await _cartRepository.RemoveFromCartAsync(
                    userId,
                    productId);

            if (!removed)
            {
                throw new InvalidOperationException(
                    "The cart item could not be removed.");
            }

            return true;
        }

        // --------------------------------------------------
        // CLEAR CART
        // --------------------------------------------------
        public async Task ClearCartAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            await _cartRepository.ClearCartAsync(userId);
        }

        // --------------------------------------------------
        // COMMON ID VALIDATION
        // --------------------------------------------------
        private static void ValidateIds(
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
        }
    }
}