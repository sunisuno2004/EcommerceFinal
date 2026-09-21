using System.Security.Claims;
using ECommerce.API.DTOs;
using ECommerce.Business.Exceptions;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            IOrderService orderService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _orderService = orderService;
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
        // PROCESS PAYMENT
        // POST: api/Payment/{orderId}
        // --------------------------------------------------
        [HttpPost("{orderId}")]
        public async Task<IActionResult> MakePayment(
            int orderId,
            PaymentDto dto)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Payment attempt for order {OrderId} " +
                "by user {UserId}.",
                orderId,
                userId);

            // This also verifies that the order
            // belongs to the logged-in user.
            var order =
                await _orderService.GetOrderAsync(
                    orderId,
                    userId);

            if (!string.Equals(
                    order.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(
                    "Payment cannot be processed because " +
                    "the order is not in Pending status.");
            }

            var success =
                await _paymentService.ProcessPaymentAsync(
                    orderId,
                    dto.PaymentMethod);

            if (!success)
            {
                throw new InvalidOperationException(
                    "Payment processing failed.");
            }

            var updatedOrder =
                await _orderService.UpdateOrderStatusAsync(
                    orderId,
                    "Confirmed");

            _logger.LogInformation(
                "Payment completed successfully for order {OrderId}.",
                orderId);

            return Ok(new
            {
                message = "Payment successful.",
                orderId = updatedOrder.Id,
                paymentMethod = dto.PaymentMethod,
                status = updatedOrder.Status
            });
        }
    }
}