using ECommerce.Business.Models;

namespace ECommerce.Business.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync();

        Task<Product?> GetProductByIdAsync(int id);

        Task<Product> AddProductAsync(Product product);

        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<List<Product>> AddProductsAsync(List<Product> products);
        Task<bool> DeleteProductAsync(int id);
    }
}