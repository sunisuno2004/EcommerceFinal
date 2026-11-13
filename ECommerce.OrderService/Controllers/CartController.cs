using ECommerce.OrderService.DTOs;
using ECommerce.OrderService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.OrderService.Exceptions;
using System.Security.Claims;
namespace ECommerce.OrderService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // --------------------------------------------------
        // GET LOGGED-IN USER ID FROM JWT
        // --------------------------------------------------
        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new UnauthorizedException(
                    "User ID was not found in the authentication token.");
            }

            if (!int.TryParse(
                    userIdClaim,
                    out var userId))
            {
                throw new UnauthorizedException(
                    "Invalid user ID in the authentication token.");
            }

            if (userId <= 0)
            {
                throw new UnauthorizedException(
                    "Invalid user ID in the authentication token.");
            }

            return userId;
        }

        // --------------------------------------------------
        // GET CART
        // GET: api/Cart
        // --------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Getting cart for user {UserId}.",
                userId);

            var cart =
                await _cartService.GetCartAsync(userId);

            _logger.LogInformation(
                "Retrieved {Count} cart items for user {UserId}.",
                cart.Count,
                userId);

            return Ok(cart);
        }

        // --------------------------------------------------
        // ADD TO CART
        // POST: api/Cart
        // --------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddToCart(
            AddToCartDto dto)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Adding product {ProductId} to cart " +
                "for user {UserId}.",
                dto.ProductId,
                userId);

            var cartItem =
                await _cartService.AddToCartAsync(
                    userId,
                    dto.ProductId,
                    dto.Quantity);

            _logger.LogInformation(
                "Product {ProductId} added to cart " +
                "for user {UserId}.",
                dto.ProductId,
                userId);

            return Ok(cartItem);
        }

        // --------------------------------------------------
        // UPDATE CART ITEM
        // PUT: api/Cart/{productId}
        // --------------------------------------------------
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCart(
            int productId,
            UpdateCartDto dto)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Updating product {ProductId} in cart " +
                "for user {UserId}.",
                productId,
                userId);

            var cartItem =
                await _cartService.UpdateCartItemAsync(
                    userId,
                    productId,
                    dto.Quantity);

            _logger.LogInformation(
                "Product {ProductId} updated in cart " +
                "for user {UserId}.",
                productId,
                userId);

            return Ok(cartItem);
        }

        // --------------------------------------------------
        // REMOVE ITEM FROM CART
        // DELETE: api/Cart/{productId}
        // --------------------------------------------------
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(
            int productId)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Removing product {ProductId} from cart " +
                "for user {UserId}.",
                productId,
                userId);

            await _cartService.RemoveFromCartAsync(
                userId,
                productId);

            _logger.LogInformation(
                "Product {ProductId} removed from cart " +
                "for user {UserId}.",
                productId,
                userId);

            return NoContent();
        }

        // --------------------------------------------------
        // CLEAR CART
        // DELETE: api/Cart
        // --------------------------------------------------
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Clearing cart for user {UserId}.",
                userId);

            await _cartService.ClearCartAsync(userId);

            _logger.LogInformation(
                "Cart cleared for user {UserId}.",
                userId);

            return NoContent();
        }
    }
}