using System.ComponentModel.DataAnnotations;

namespace ECommerce.ProductService.DTOs
{
    public class UpdateProductBatchDto
    {
        [Required]
        [StringLength(50)]
        public required string BatchNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}