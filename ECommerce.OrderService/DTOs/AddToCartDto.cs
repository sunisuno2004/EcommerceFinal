using System.ComponentModel.DataAnnotations;

namespace ECommerce.OrderService.DTOs
{
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}