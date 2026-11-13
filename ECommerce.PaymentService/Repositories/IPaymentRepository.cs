using ECommerce.PaymentService.Models;

namespace ECommerce.PaymentService.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);

        Task<Payment?> GetPaymentByIdAsync(int id);

        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);

        Task<Payment?> UpdateStatusAsync(
            int id,
            string status,
            string? transactionReference);
    }
}