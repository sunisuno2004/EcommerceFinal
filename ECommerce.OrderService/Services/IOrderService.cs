using ECommerce.OrderService.Models;

namespace ECommerce.OrderService.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int userId);

        Task<List<Order>> GetMyOrdersAsync(
            int userId);

        Task<Order> GetOrderAsync(
            int orderId,
            int userId);

        Task<List<Order>> GetAllOrdersAsync();

        Task<Order> UpdateOrderStatusAsync(
            int orderId,
            string status);
    }
}