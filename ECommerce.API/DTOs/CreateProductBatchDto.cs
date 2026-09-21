using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class CreateProductBatchDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public required string BatchNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}