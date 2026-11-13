using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductBatchRepository _batchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IProductBatchRepository batchRepository,
    IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _batchRepository = batchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cart =
                    await _cartRepository.GetCartAsync(userId);

                if (cart.Count == 0)
                {
                    throw new ConflictException(
                        "Cart is empty.");
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending"
                };

                decimal total = 0;

                foreach (var cartItem in cart)
                {
                    var product =
                        await _productRepository
                            .GetProductByIdAsync(
                                cartItem.ProductId);

                    if (product is null)
                    {
                        throw new NotFoundException(
                            $"Product with ID " +
                            $"{cartItem.ProductId} was not found.");
                    }

                    var batches =
                        await _batchRepository
                            .GetAvailableBatchesForProductAsync(
                                product.Id);

                    var availableBatchStock =
                        batches.Sum(b => b.Quantity);

                    if (availableBatchStock <
                        cartItem.Quantity)
                    {
                        throw new ConflictException(
                            $"Insufficient stock for " +
                            $"'{product.Name}'.");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price
                    };

                    order.OrderItems.Add(orderItem);

                    total +=
                        product.Price *
                        cartItem.Quantity;

                    var remainingQuantity =
                        cartItem.Quantity;

                    foreach (var batch in batches)
                    {
                        if (remainingQuantity == 0)
                        {
                            break;
                        }

                        var quantityToDeduct =
                            Math.Min(
                                batch.Quantity,
                                remainingQuantity);

                        batch.Quantity -=
                            quantityToDeduct;

                        remainingQuantity -=
                            quantityToDeduct;

                        var updatedBatch =
                            await _batchRepository
                                .UpdateBatchAsync(
                                    batch.Id,
                                    batch);

                        if (updatedBatch is null)
                        {
                            throw new NotFoundException(
                                $"Batch with ID {batch.Id} " +
                                "could not be updated.");
                        }
                    }

                    var remainingStock =
    await _batchRepository
        .GetTotalStockByProductIdAsync(
            product.Id);

                    var updatedProduct =
                        await _productRepository
                            .UpdateStockQuantityAsync(
                                product.Id,
                                remainingStock);

                    if (updatedProduct is null)
                    {
                        throw new NotFoundException(
                            $"Product with ID {product.Id} " +
                            "could not be updated.");
                    }
                }

                order.TotalAmount = total;

                var createdOrder =
                    await _orderRepository
                        .CreateOrderAsync(order);

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

        public async Task<List<Order>> GetMyOrdersAsync(
            int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            return await _orderRepository
                .GetOrdersByUserAsync(userId);
        }

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
                    $"Order with ID {orderId} was not found.");
            }

            return order;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository
                .GetAllOrdersAsync();
        }

        public async Task<Order> UpdateOrderStatusAsync(
            int orderId,
            string status)
        {
            if (orderId <= 0)
            {
                throw new ValidationException(
                    "Order ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ValidationException(
                    "Order status is required.");
            }

            var allowedStatuses = new[]
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
                allowedStatuses.First(s =>
                    string.Equals(
                        s,
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
                    $"Order with ID {orderId} was not found.");
            }

            return order;
        }
    }
}