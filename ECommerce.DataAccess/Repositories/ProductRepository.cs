using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _context;

        public ProductRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateProductAsync(
            int id,
            Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
           

            await _context.SaveChangesAsync();

            return existingProduct;
        }
        public async Task<List<Product>> AddProductsAsync(List<Product> products)
        {
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return products;
        }
        public async Task<Product?> UpdateStockQuantityAsync(
    int productId,
    int stockQuantity)
        {
            var product =
                await _context.Products.FindAsync(productId);

            if (product is null)
            {
                return null;
            }

            product.StockQuantity = stockQuantity;

            await _context.SaveChangesAsync();

            return product;
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}