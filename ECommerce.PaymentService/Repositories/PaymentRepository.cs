using ECommerce.PaymentService.Data;
using ECommerce.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.PaymentService.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(
            PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(
            Payment payment)
        {
            await _context.Payments.AddAsync(payment);

            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment?> GetPaymentByIdAsync(
            int id)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(
                    p => p.Id == id);
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(
            int orderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(
                    p => p.OrderId == orderId);
        }

        public async Task<Payment?> UpdateStatusAsync(
            int id,
            string status,
            string? transactionReference)
        {
            var payment =
                await _context.Payments
                    .FirstOrDefaultAsync(
                        p => p.Id == id);

            if (payment is null)
            {
                return null;
            }

            payment.Status = status;
            payment.TransactionReference =
                transactionReference;

            await _context.SaveChangesAsync();

            return payment;
        }
    }
}