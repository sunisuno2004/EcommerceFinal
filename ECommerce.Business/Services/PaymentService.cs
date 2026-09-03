namespace ECommerce.Business.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPaymentAsync(
            int orderId,
            string paymentMethod)
        {
            await Task.Delay(100);

            var allowedMethods = new[]
            {
                "UPI",
                "Card",
                "CashOnDelivery"
            };

            if (!allowedMethods.Contains(
                paymentMethod,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Invalid payment method.");
            }

            // Mock payment processing
            return true;
        }
    }
}