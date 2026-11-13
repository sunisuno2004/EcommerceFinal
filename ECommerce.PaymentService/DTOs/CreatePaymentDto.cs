using System.ComponentModel.DataAnnotations;

namespace ECommerce.PaymentService.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }
            = string.Empty;
    }
}