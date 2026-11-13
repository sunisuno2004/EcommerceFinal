namespace ECommerce.ProductService.Services
{
    public interface IInventoryService
    {
        Task<int> GetAvailableStockAsync(
            int productId);
        Task DeductStockAsync(
    int productId,
    int quantity);
    }
}