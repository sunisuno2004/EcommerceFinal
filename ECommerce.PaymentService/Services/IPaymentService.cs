using ECommerce.PaymentService.Models;

namespace ECommerce.PaymentService.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(
            int orderId,
            int userId,
            decimal amount,
            string paymentMethod);

        Task<Payment> GetPaymentByIdAsync(
            int id,
            int userId);

        Task<Payment> GetPaymentByOrderIdAsync(
            int orderId,
            int userId);

        Task<Payment> UpdatePaymentStatusAsync(
            int id,
            string status,
            string? transactionReference);
    }
}