using ECommerce.OrderService.Data;
using ECommerce.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(
            Order order)
        {
            await _context.Orders
                .AddAsync(order);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>>
            GetOrdersByUserAsync(
                int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?>
            GetOrderByIdAsync(
                int orderId,
                int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(
                    o =>
                        o.Id == orderId &&
                        o.UserId == userId);
        }

        public async Task<List<Order>>
            GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?>
            UpdateOrderStatusAsync(
                int orderId,
                string status)
        {
            var order =
                await _context.Orders
                    .FirstOrDefaultAsync(
                        o => o.Id == orderId);

            if (order is null)
            {
                return null;
            }

            order.Status = status;

            await _context.SaveChangesAsync();

            return order;
        }
    }
}