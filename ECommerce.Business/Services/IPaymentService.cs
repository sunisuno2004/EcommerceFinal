namespace ECommerce.Business.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(
            int orderId,
            string paymentMethod);
    }
}