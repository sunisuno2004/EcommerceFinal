using System.ComponentModel.DataAnnotations;

namespace ECommerce.ProductService.DTOs
{
    public class CreateProductBatchDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public required string BatchNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}