using ECommerce.ProductService.Models;

namespace ECommerce.ProductService.Services
{
    public interface IProductService
    {
        Task<List<Product>>
            GetProductsAsync();

        Task<Product>
            GetProductByIdAsync(
                int id);

        Task<Product>
            AddProductAsync(
                Product product);

        Task<Product>
            UpdateProductAsync(
                int id,
                Product product);

        Task<List<Product>>
            AddProductsAsync(
                List<Product> products);

        Task DeleteProductAsync(
            int id);
    }
}