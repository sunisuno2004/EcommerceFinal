using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class UpdateCartDto
    {
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}