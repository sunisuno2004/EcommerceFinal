using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class PaymentDto
    {
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}