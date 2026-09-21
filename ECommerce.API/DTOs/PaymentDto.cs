using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class PaymentDto
    {
        [Required]
        public required string PaymentMethod { get; set; }
    }
}