using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(
            Order order);

        Task<List<Order>> GetOrdersByUserAsync(
            int userId);

        Task<Order?> GetOrderByIdAsync(
            int orderId,
            int userId);

        Task<List<Order>> GetAllOrdersAsync();

        Task<Order?> UpdateOrderStatusAsync(
            int orderId,
            string status);
    }
}