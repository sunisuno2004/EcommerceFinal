using ECommerce.Business.Exceptions;

namespace ECommerce.Business.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPaymentAsync(
            int orderId,
            string paymentMethod)
        {
            if (orderId <= 0)
            {
                throw new ValidationException(
                    "Order ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ValidationException(
                    "Payment method is required.");
            }

            var allowedMethods = new[]
            {
                "UPI",
                "Card",
                "CashOnDelivery"
            };

            var normalizedPaymentMethod =
                allowedMethods.FirstOrDefault(
                    method =>
                        string.Equals(
                            method,
                            paymentMethod.Trim(),
                            StringComparison.OrdinalIgnoreCase));

            if (normalizedPaymentMethod is null)
            {
                throw new ValidationException(
                    "Invalid payment method. " +
                    "Allowed methods are: UPI, Card, CashOnDelivery.");
            }

            // Simulate payment processing.
            await Task.Delay(100);

            return true;
        }
    }
}