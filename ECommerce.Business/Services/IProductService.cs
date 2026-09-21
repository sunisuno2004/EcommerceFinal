using ECommerce.Business.Models;

namespace ECommerce.Business.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();

        Task<Product> GetProductByIdAsync(int id);

        Task<Product> AddProductAsync(Product product);

        Task<List<Product>> AddProductsAsync(
            List<Product> products);

        Task<Product> UpdateProductAsync(
            int id,
            Product product);

        Task<bool> DeleteProductAsync(int id);
    }
}