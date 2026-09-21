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
        // CREATE ORDER
        // POST: api/Orders
        // --------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Creating order for user {UserId}.",
                userId);

            var order =
                await _orderService.CreateOrderAsync(
                    userId);

            _logger.LogInformation(
                "Order {OrderId} created successfully " +
                "for user {UserId}.",
                order.Id,
                userId);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                order);
        }

        // --------------------------------------------------
        // GET LOGGED-IN USER'S ORDERS
        // GET: api/Orders
        // --------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Getting orders for user {UserId}.",
                userId);

            var orders =
                await _orderService.GetMyOrdersAsync(
                    userId);

            _logger.LogInformation(
                "Retrieved {Count} orders for user {UserId}.",
                orders.Count,
                userId);

            return Ok(orders);
        }

        // --------------------------------------------------
        // GET SINGLE ORDER
        // GET: api/Orders/5
        // --------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Getting order {OrderId} for user {UserId}.",
                id,
                userId);

            var order =
                await _orderService.GetOrderAsync(
                    id,
                    userId);

            return Ok(order);
        }

        // --------------------------------------------------
        // GET ALL ORDERS
        // GET: api/Orders/admin/all
        // ADMIN ONLY
        // --------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllOrders()
        {
            _logger.LogInformation(
                "Admin requested all orders.");

            var orders =
                await _orderService.GetAllOrdersAsync();

            _logger.LogInformation(
                "Retrieved {Count} orders.",
                orders.Count);

            return Ok(orders);
        }

        // --------------------------------------------------
        // UPDATE ORDER STATUS
        // PUT: api/Orders/5/status
        // ADMIN ONLY
        // --------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            _logger.LogInformation(
                "Updating order {OrderId} status to {Status}.",
                id,
                dto.Status);

            var order =
                await _orderService.UpdateOrderStatusAsync(
                    id,
                    dto.Status);

            _logger.LogInformation(
                "Order {OrderId} status changed to {Status}.",
                order.Id,
                order.Status);

            return Ok(order);
        }
    }
}