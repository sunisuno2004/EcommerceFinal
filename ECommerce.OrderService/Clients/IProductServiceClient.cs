namespace ECommerce.OrderService.Clients
{
    public interface IProductServiceClient
    {
        Task<ProductResponse?> GetProductAsync(
            int productId);

        Task<int> GetAvailableStockAsync(
            int productId);

        Task DeductStockAsync(
            int productId,
            int quantity);
    }

    public class ProductResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
    }
}