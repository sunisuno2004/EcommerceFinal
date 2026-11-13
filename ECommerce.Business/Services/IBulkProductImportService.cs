using ECommerce.Business.Models;

namespace ECommerce.Business.Services
{
    public interface IBulkProductImportService
    {
        Task<List<Product>> ImportAsync(
            Stream excelStream);
    }
}