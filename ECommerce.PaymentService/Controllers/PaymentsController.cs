using ECommerce.PaymentService.DTOs;
using ECommerce.PaymentService.Exceptions;
using ECommerce.PaymentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService
            _paymentService;

        public PaymentsController(
            IPaymentService paymentService)
        {
            _paymentService =
                paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePaymentDto dto)
        {
            var userId =
                GetUserId();

            var payment =
                await _paymentService
                    .CreatePaymentAsync(
                        dto.OrderId,
                        userId,
                        dto.Amount,
                        dto.PaymentMethod);

            return Ok(payment);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var userId =
                GetUserId();

            var payment =
                await _paymentService
                    .GetPaymentByIdAsync(
                        id,
                        userId);

            return Ok(payment);
        }

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetByOrder(
            int orderId)
        {
            var userId =
                GetUserId();

            var payment =
                await _paymentService
                    .GetPaymentByOrderIdAsync(
                        orderId,
                        userId);

            return Ok(payment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            UpdatePaymentStatusDto dto)
        {
            var payment =
                await _paymentService
                    .UpdatePaymentStatusAsync(
                        id,
                        dto.Status,
                        dto.TransactionReference);

            return Ok(payment);
        }

        private int GetUserId()
        {
            var value =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    value,
                    out var userId))
            {
                throw new UnauthorizedException(
                    "Invalid user identity.");
            }

            return userId;
        }
    }
}