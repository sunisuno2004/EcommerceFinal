using ECommerce.API.DTOs;
using ECommerce.Business.Models;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductBatchesController : ControllerBase
    {
        private readonly IProductBatchService _batchService;
        private readonly ILogger<ProductBatchesController> _logger;

        public ProductBatchesController(
            IProductBatchService batchService,
            ILogger<ProductBatchesController> logger)
        {
            _batchService = batchService;
            _logger = logger;
        }

        // --------------------------------------------------
        // GET ALL BATCHES
        // GET: api/ProductBatches
        // --------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllBatches()
        {
            _logger.LogInformation(
                "Getting all product batches.");

            var batches =
                await _batchService.GetAllBatchesAsync();

            _logger.LogInformation(
                "Retrieved {Count} product batches.",
                batches.Count);

            return Ok(batches);
        }

        // --------------------------------------------------
        // GET BATCH BY ID
        // GET: api/ProductBatches/1
        // --------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatch(int id)
        {
            _logger.LogInformation(
                "Getting batch with ID {BatchId}.",
                id);

            var batch =
                await _batchService.GetBatchByIdAsync(id);

            return Ok(batch);
        }

        // --------------------------------------------------
        // GET BATCHES BY PRODUCT ID
        // GET: api/ProductBatches/product/1
        // --------------------------------------------------
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetBatchesByProduct(
            int productId)
        {
            _logger.LogInformation(
                "Getting batches for product {ProductId}.",
                productId);

            var batches =
                await _batchService
                    .GetBatchesByProductIdAsync(productId);

            _logger.LogInformation(
                "Retrieved {Count} batches for product {ProductId}.",
                batches.Count,
                productId);

            return Ok(batches);
        }

        // --------------------------------------------------
        // CREATE BATCH
        // POST: api/ProductBatches
        // ADMIN ONLY
        // --------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBatch(
            CreateProductBatchDto dto)
        {
            _logger.LogInformation(
                "Creating batch {BatchNumber} for product {ProductId}.",
                dto.BatchNumber,
                dto.ProductId);

            var batch = new ProductBatch
            {
                ProductId = dto.ProductId,
                BatchNumber = dto.BatchNumber,
                Quantity = dto.Quantity,
                ManufacturingDate = dto.ManufacturingDate,
                ExpiryDate = dto.ExpiryDate,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdBatch =
                await _batchService.AddBatchAsync(batch);

            _logger.LogInformation(
                "Batch {BatchId} ({BatchNumber}) created " +
                "for product {ProductId}.",
                createdBatch.Id,
                createdBatch.BatchNumber,
                createdBatch.ProductId);

            return CreatedAtAction(
                nameof(GetBatch),
                new { id = createdBatch.Id },
                createdBatch);
        }

        // --------------------------------------------------
        // UPDATE BATCH
        // PUT: api/ProductBatches/1
        // ADMIN ONLY
        // --------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(
            int id,
            UpdateProductBatchDto dto)
        {
            _logger.LogInformation(
                "Updating batch with ID {BatchId}.",
                id);

            var batch = new ProductBatch
            {
                BatchNumber = dto.BatchNumber,
                Quantity = dto.Quantity,
                ManufacturingDate = dto.ManufacturingDate,
                ExpiryDate = dto.ExpiryDate,
                IsActive = dto.IsActive
            };

            var updatedBatch =
                await _batchService.UpdateBatchAsync(
                    id,
                    batch);

            _logger.LogInformation(
                "Batch {BatchId} updated successfully.",
                updatedBatch.Id);

            return Ok(updatedBatch);
        }

        // --------------------------------------------------
        // DELETE BATCH
        // DELETE: api/ProductBatches/1
        // ADMIN ONLY
        // --------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            _logger.LogInformation(
                "Deleting batch with ID {BatchId}.",
                id);

            await _batchService.DeleteBatchAsync(id);

            _logger.LogInformation(
                "Batch {BatchId} deleted successfully.",
                id);

            return NoContent();
        }
    }
}