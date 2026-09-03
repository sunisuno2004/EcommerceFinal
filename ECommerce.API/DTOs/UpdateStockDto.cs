using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs
{
    public class UpdateStockDto
    {
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }
}