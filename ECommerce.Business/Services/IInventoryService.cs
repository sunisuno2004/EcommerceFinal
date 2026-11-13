namespace ECommerce.Business.Services
{
    public interface IInventoryService
    {
        Task<int> GetAvailableStockAsync(
            int productId);
    }
}