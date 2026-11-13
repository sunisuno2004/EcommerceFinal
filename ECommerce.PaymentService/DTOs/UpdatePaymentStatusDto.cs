using System.ComponentModel.DataAnnotations;

namespace ECommerce.PaymentService.DTOs
{
    public class UpdatePaymentStatusDto
    {
        [Required]
        public string Status { get; set; }
            = string.Empty;

        public string? TransactionReference
        {
            get;
            set;
        }
    }
}