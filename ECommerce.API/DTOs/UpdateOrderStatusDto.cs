using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public required string Status { get; set; }
    }
}