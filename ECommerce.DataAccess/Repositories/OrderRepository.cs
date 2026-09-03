using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _context;

        public OrderRepository(
            ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(
            Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserAsync(
            int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(
            int orderId,
            int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.UserId == userId);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> UpdateOrderStatusAsync(
            int orderId,
            string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            order.Status = status;

            await _context.SaveChangesAsync();

            return order;
        }
    }
}