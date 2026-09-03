using System.Security.Claims;
using ECommerce.API.DTOs;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
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

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException(
                    "User ID not found in token.");
            }

            return int.Parse(userIdClaim);
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            var cart = await _cartService.GetCartAsync(userId);

            return Ok(cart);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(
            AddToCartDto dto)
        {
            var userId = GetUserId();

            var cartItem = await _cartService.AddToCartAsync(
                userId,
                dto.ProductId,
                dto.Quantity);

            _logger.LogInformation(
                "Product {ProductId} added to cart for user {UserId}.",
                dto.ProductId,
                userId);

            return Ok(cartItem);
        }

        // PUT: api/cart/{productId}
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCart(
            int productId,
            UpdateCartDto dto)
        {
            var userId = GetUserId();

            var cartItem =
                await _cartService.UpdateCartItemAsync(
                    userId,
                    productId,
                    dto.Quantity);

            if (cartItem == null)
            {
                return NotFound(new
                {
                    message = "Cart item not found."
                });
            }

            return Ok(cartItem);
        }

        // DELETE: api/cart/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(
            int productId)
        {
            var userId = GetUserId();

            var removed =
                await _cartService.RemoveFromCartAsync(
                    userId,
                    productId);

            if (!removed)
            {
                return NotFound(new
                {
                    message = "Cart item not found."
                });
            }

            return NoContent();
        }

        // DELETE: api/cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            await _cartService.ClearCartAsync(userId);

            return NoContent();
        }
    }
}