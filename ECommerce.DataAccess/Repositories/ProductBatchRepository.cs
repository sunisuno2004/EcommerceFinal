using ECommerce.Business.Models;
using ECommerce.Business.Repositories;
using ECommerce.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repositories
{
    public class ProductBatchRepository : IProductBatchRepository
    {
        private readonly ECommerceDbContext _context;

        public ProductBatchRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductBatch>> GetAllBatchesAsync()
        {
            return await _context.ProductBatches
                .Include(b => b.Product)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductBatch?> GetBatchByIdAsync(int id)
        {
            return await _context.ProductBatches
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<ProductBatch>> GetBatchesByProductIdAsync(
            int productId)
        {
            return await _context.ProductBatches
                .Include(b => b.Product)
                .Where(b => b.ProductId == productId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<ProductBatch>>
    GetAvailableBatchesForProductAsync(
        int productId)
        {
            var now = DateTime.UtcNow;

            return await _context.ProductBatches
                .Where(batch =>
                    batch.ProductId == productId &&
                    batch.IsActive &&
                    batch.Quantity > 0 &&
                    (
                        batch.ExpiryDate == null ||
                        batch.ExpiryDate > now
                    ))
                .OrderBy(batch =>
                    batch.ExpiryDate ?? DateTime.MaxValue)
                .ThenBy(batch => batch.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductBatch> AddBatchAsync(ProductBatch batch)
        {
            _context.ProductBatches.Add(batch);

            await _context.SaveChangesAsync();

            return batch;
        }

        public async Task<ProductBatch?> UpdateBatchAsync(
            int id,
            ProductBatch batch)
        {
            var existingBatch =
                await _context.ProductBatches.FindAsync(id);

            if (existingBatch == null)
                return null;

            existingBatch.BatchNumber = batch.BatchNumber;
            existingBatch.Quantity = batch.Quantity;
            existingBatch.ManufacturingDate =
                batch.ManufacturingDate;
            existingBatch.ExpiryDate =
                batch.ExpiryDate;
            existingBatch.IsActive =
                batch.IsActive;

            await _context.SaveChangesAsync();

            return existingBatch;
        }
        public async Task<int> GetTotalStockByProductIdAsync(
    int productId)
        {
            var now = DateTime.UtcNow;

            return await _context.ProductBatches
                .Where(batch =>
                    batch.ProductId == productId &&
                    batch.IsActive &&
                    batch.Quantity > 0 &&
                    (
                        batch.ExpiryDate == null ||
                        batch.ExpiryDate > now
                    ))
                .SumAsync(batch => batch.Quantity);
        }
        public async Task<bool> DeleteBatchAsync(int id)
        {
            var batch =
                await _context.ProductBatches.FindAsync(id);

            if (batch == null)
                return false;

            _context.ProductBatches.Remove(batch);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}