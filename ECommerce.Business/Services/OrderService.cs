using ECommerce.Business.Models;
using ECommerce.Business.Repositories;

namespace ECommerce.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> CreateOrderAsync(int userId)
        {
            var cart =
                await _cartRepository.GetCartAsync(userId);

            if (cart.Count == 0)
            {
                throw new InvalidOperationException(
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

                if (product == null)
                {
                    throw new KeyNotFoundException(
                        $"Product {cartItem.ProductId} not found.");
                }

                if (product.StockQuantity <
                    cartItem.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for {product.Name}.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);

                total +=
                    product.Price * cartItem.Quantity;

                product.StockQuantity -=
                    cartItem.Quantity;
            }

            order.TotalAmount = total;

            var createdOrder =
                await _orderRepository
                    .CreateOrderAsync(order);

            await _cartRepository.ClearCartAsync(userId);

            return createdOrder;
        }

        public async Task<List<Order>> GetMyOrdersAsync(
            int userId)
        {
            return await _orderRepository
                .GetOrdersByUserAsync(userId);
        }

        public async Task<Order?> GetOrderAsync(
            int orderId,
            int userId)
        {
            return await _orderRepository
                .GetOrderByIdAsync(
                    orderId,
                    userId);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository
                .GetAllOrdersAsync();
        }

        public async Task<Order?> UpdateOrderStatusAsync(
            int orderId,
            string status)
        {
            var allowedStatuses = new[]
            {
                "Pending",
                "Confirmed",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(status))
            {
                throw new ArgumentException(
                    "Invalid order status.");
            }

            return await _orderRepository
                .UpdateOrderStatusAsync(
                    orderId,
                    status);
        }
    }
}