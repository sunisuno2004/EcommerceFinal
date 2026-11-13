using System.ComponentModel.DataAnnotations;

namespace ECommerce.OrderService.DTOs
{
    public class UpdateCartDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}