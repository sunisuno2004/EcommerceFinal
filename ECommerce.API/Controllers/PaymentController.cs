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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentController(
            IPaymentService paymentService,
            IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException(
                    "User ID not found.");

            return int.Parse(userIdClaim);
        }

        [HttpPost("{orderId}")]
        public async Task<IActionResult> MakePayment(
            int orderId,
            PaymentDto dto)
        {
            var userId = GetUserId();

            // Verify that the order belongs to this user
            var order =
                await _orderService.GetOrderAsync(
                    orderId,
                    userId);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found."
                });
            }

            if (order.Status != "Pending")
            {
                return BadRequest(new
                {
                    message = "Payment cannot be processed for this order."
                });
            }

            var success =
                await _paymentService.ProcessPaymentAsync(
                    orderId,
                    dto.PaymentMethod);

            if (!success)
            {
                return BadRequest(new
                {
                    message = "Payment failed."
                });
            }

            // Payment successful
            await _orderService.UpdateOrderStatusAsync(
                orderId,
                "Confirmed");

            return Ok(new
            {
                message = "Payment successful.",
                orderId = orderId,
                paymentMethod = dto.PaymentMethod,
                status = "Confirmed"
            });
        }
    }
}