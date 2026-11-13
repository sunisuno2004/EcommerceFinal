using System.ComponentModel.DataAnnotations;

namespace ECommerce.OrderService.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; }
            = string.Empty;
    }
}