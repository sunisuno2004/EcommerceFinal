using ECommerce.PaymentService.Exceptions;
using ECommerce.PaymentService.Models;
using ECommerce.PaymentService.Repositories;

namespace ECommerce.PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(
            IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // --------------------------------------------------
        // CREATE PAYMENT
        // --------------------------------------------------
        public async Task<Payment> CreatePaymentAsync(
            int orderId,
            int userId,
            decimal amount,
            string paymentMethod)
        {
            if (orderId <= 0)
            {
                throw new ValidationException(
                    "Order ID must be greater than zero.");
            }

            if (userId <= 0)
            {
                throw new ValidationException(
                    "User ID must be greater than zero.");
            }

            if (amount <= 0)
            {
                throw new ValidationException(
                    "Amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ValidationException(
                    "Payment method is required.");
            }

            var existingPayment =
                await _paymentRepository
                    .GetPaymentByOrderIdAsync(orderId);

            if (existingPayment is not null)
            {
                throw new ConflictException(
                    "A payment already exists for this order.");
            }

            var normalizedPaymentMethod =
                NormalizePaymentMethod(paymentMethod);

            var isImmediatePayment =
                normalizedPaymentMethod == "UPI" ||
                normalizedPaymentMethod == "Card";

            var payment = new Payment
            {
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                PaymentMethod = normalizedPaymentMethod,

                Status = isImmediatePayment
                    ? "Completed"
                    : "Pending",

                TransactionReference =
                    isImmediatePayment
                        ? GenerateTransactionReference()
                        : null,

                CreatedAt = DateTime.UtcNow
            };

            return await _paymentRepository
                .CreatePaymentAsync(payment);
        }

        // --------------------------------------------------
        // GET PAYMENT BY ID
        // --------------------------------------------------
        public async Task<Payment> GetPaymentByIdAsync(
            int id,
            int userId)
        {
            var payment =
                await _paymentRepository
                    .GetPaymentByIdAsync(id);

            if (payment is null ||
                payment.UserId != userId)
            {
                throw new NotFoundException(
                    $"Payment with ID {id} was not found.");
            }

            return payment;
        }

        // --------------------------------------------------
        // GET PAYMENT BY ORDER ID
        // --------------------------------------------------
        public async Task<Payment> GetPaymentByOrderIdAsync(
            int orderId,
            int userId)
        {
            var payment =
                await _paymentRepository
                    .GetPaymentByOrderIdAsync(orderId);

            if (payment is null ||
                payment.UserId != userId)
            {
                throw new NotFoundException(
                    $"Payment for order {orderId} was not found.");
            }

            return payment;
        }

        // --------------------------------------------------
        // UPDATE PAYMENT STATUS
        // --------------------------------------------------
        public async Task<Payment> UpdatePaymentStatusAsync(
            int id,
            string status,
            string? transactionReference)
        {
            var allowedStatuses = new[]
            {
                "Pending",
                "Completed",
                "Failed",
                "Refunded"
            };

            if (!allowedStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Invalid payment status.");
            }

            var normalizedStatus =
                allowedStatuses.First(
                    s => string.Equals(
                        s,
                        status,
                        StringComparison.OrdinalIgnoreCase));

            var payment =
                await _paymentRepository
                    .UpdateStatusAsync(
                        id,
                        normalizedStatus,
                        transactionReference);

            if (payment is null)
            {
                throw new NotFoundException(
                    $"Payment with ID {id} was not found.");
            }

            return payment;
        }

        // --------------------------------------------------
        // NORMALIZE PAYMENT METHOD
        // --------------------------------------------------
        private static string NormalizePaymentMethod(
            string paymentMethod)
        {
            if (paymentMethod.Equals(
                    "UPI",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "UPI";
            }

            if (paymentMethod.Equals(
                    "Card",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "Card";
            }

            if (paymentMethod.Equals(
                    "CashOnDelivery",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "CashOnDelivery";
            }

            throw new ValidationException(
                "Invalid payment method. Allowed methods are UPI, Card, and CashOnDelivery.");
        }

        // --------------------------------------------------
        // GENERATE SIMULATED TRANSACTION REFERENCE
        // --------------------------------------------------
        private static string GenerateTransactionReference()
        {
            return $"SIM-{Guid.NewGuid():N}"
                .Substring(0, 16)
                .ToUpperInvariant();
        }
    }
}