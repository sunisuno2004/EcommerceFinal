using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Services
{
    public interface IBulkProductImportService
    {
        Task<List<Product>> ImportAsync(
            Stream excelStream);
    }
}