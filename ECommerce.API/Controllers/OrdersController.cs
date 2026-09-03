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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException(
                    "User ID not found in token.");

            return int.Parse(userIdClaim);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = GetUserId();

            var order = await _orderService.CreateOrderAsync(userId);

            _logger.LogInformation(
                "Order {OrderId} created for user {UserId}.",
                order.Id,
                userId);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                order);
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();

            var orders =
                await _orderService.GetMyOrdersAsync(userId);

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserId();

            var order =
                await _orderService.GetOrderAsync(id, userId);

            if (order == null)
                return NotFound(new
                {
                    message = "Order not found."
                });

            return Ok(order);
        }

        // GET: api/orders/admin/all
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders =
                await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        // PUT: api/orders/5/status
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            var order =
                await _orderService.UpdateOrderStatusAsync(
                    id,
                    dto.Status);

            if (order == null)
                return NotFound(new
                {
                    message = "Order not found."
                });

            _logger.LogInformation(
                "Order {OrderId} status changed to {Status}.",
                id,
                dto.Status);

            return Ok(order);
        }
    }
}