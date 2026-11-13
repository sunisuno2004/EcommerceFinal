using ECommerce.OrderService.Clients;
using ECommerce.OrderService.Data;
using ECommerce.OrderService.Exceptions;
using ECommerce.OrderService.Models;
using ECommerce.OrderService.Repositories;

namespace ECommerce.OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductServiceClient _productServiceClient;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductServiceClient productServiceClient,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productServiceClient = productServiceClient;
            _unitOfWork = unitOfWork;
        }

        // --------------------------------------------------
        // CREATE ORDER
        // --------------------------------------------------
        public async Task<Order> CreateOrderAsync(
            int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            await _unitOfWork
                .BeginTransactionAsync();

            try
            {
                var cart =
                    await _cartRepository
                        .GetCartAsync(userId);

                if (cart.Count == 0)
                {
                    throw new ConflictException(
                        "Cart is empty.");
                }

                var order =
                    new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.UtcNow,
                        Status = "Pending"
                    };

                decimal totalAmount = 0;

                // ------------------------------------------
                // Validate all products and stock first
                // ------------------------------------------
                foreach (var cartItem in cart)
                {
                    var product =
                        await _productServiceClient
                            .GetProductAsync(
                                cartItem.ProductId);

                    if (product is null)
                    {
                        throw new NotFoundException(
                            $"Product with ID " +
                            $"{cartItem.ProductId} " +
                            "was not found.");
                    }

                    var availableStock =
                        await _productServiceClient
                            .GetAvailableStockAsync(
                                cartItem.ProductId);

                    if (availableStock <
                        cartItem.Quantity)
                    {
                        throw new ConflictException(
                            $"Insufficient stock for " +
                            $"'{product.Name}'. " +
                            $"Available stock: " +
                            $"{availableStock}.");
                    }

                    var orderItem =
                        new OrderItem
                        {
                            ProductId =
                                product.Id,

                            Quantity =
                                cartItem.Quantity,

                            UnitPrice =
                                product.Price
                        };

                    order.OrderItems.Add(
                        orderItem);

                    totalAmount +=
                        product.Price *
                        cartItem.Quantity;
                }

                // ------------------------------------------
                // Ask ProductService to deduct inventory
                // ------------------------------------------
                foreach (var cartItem in cart)
                {
                    await _productServiceClient
                        .DeductStockAsync(
                            cartItem.ProductId,
                            cartItem.Quantity);
                }

                order.TotalAmount =
                    totalAmount;

                // ------------------------------------------
                // Save order
                // ------------------------------------------
                var createdOrder =
                    await _orderRepository
                        .CreateOrderAsync(order);

                // ------------------------------------------
                // Clear cart
                // ------------------------------------------
                await _cartRepository
                    .ClearCartAsync(userId);

                await _unitOfWork
                    .CommitTransactionAsync();

                return createdOrder;
            }
            catch
            {
                await _unitOfWork
                    .RollbackTransactionAsync();

                throw;
            }
        }

        // --------------------------------------------------
        // GET LOGGED-IN USER ORDERS
        // --------------------------------------------------
        public async Task<List<Order>>
            GetMyOrdersAsync(
                int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            return await _orderRepository
                .GetOrdersByUserAsync(
                    userId);
        }

        // --------------------------------------------------
        // GET ONE ORDER
        // --------------------------------------------------
        public async Task<Order> GetOrderAsync(
            int orderId,
            int userId)
        {
            if (orderId <= 0)
            {
                throw new ValidationException(
                    "Order ID must be greater than zero.");
            }

            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            var order =
                await _orderRepository
                    .GetOrderByIdAsync(
                        orderId,
                        userId);

            if (order is null)
            {
                throw new NotFoundException(
                    $"Order with ID {orderId} " +
                    "was not found.");
            }

            return order;
        }

        // --------------------------------------------------
        // GET ALL ORDERS - ADMIN
        // --------------------------------------------------
        public async Task<List<Order>>
            GetAllOrdersAsync()
        {
            return await _orderRepository
                .GetAllOrdersAsync();
        }

        // --------------------------------------------------
        // UPDATE ORDER STATUS - ADMIN
        // --------------------------------------------------
        public async Task<Order>
            UpdateOrderStatusAsync(
                int orderId,
                string status)
        {
            if (orderId <= 0)
            {
                throw new ValidationException(
                    "Order ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(
                    status))
            {
                throw new ValidationException(
                    "Order status is required.");
            }

            var allowedStatuses =
                new[]
                {
                    "Pending",
                    "Confirmed",
                    "Shipped",
                    "Delivered",
                    "Cancelled"
                };

            if (!allowedStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Invalid order status.");
            }

            var normalizedStatus =
                allowedStatuses.First(
                    allowed =>
                        string.Equals(
                            allowed,
                            status,
                            StringComparison.OrdinalIgnoreCase));

            var order =
                await _orderRepository
                    .UpdateOrderStatusAsync(
                        orderId,
                        normalizedStatus);

            if (order is null)
            {
                throw new NotFoundException(
                    $"Order with ID {orderId} " +
                    "was not found.");
            }

            return order;
        }
    }
}