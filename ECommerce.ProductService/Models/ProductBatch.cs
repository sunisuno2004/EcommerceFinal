using System.ComponentModel.DataAnnotations;

namespace ECommerce.ProductService.Models
{
    public class ProductBatch
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public required string BatchNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public bool IsActive { get; set; }
            = true;

        public Product? Product { get; set; }
    }
}