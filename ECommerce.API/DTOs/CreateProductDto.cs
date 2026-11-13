using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0.01, 999999999)]
        public decimal Price { get; set; }

        
    }
}