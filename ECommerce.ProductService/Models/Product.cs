using System.ComponentModel.DataAnnotations;

namespace ECommerce.ProductService.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0.01, 999999999)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public bool IsActive { get; set; }
            = true;
    }
}