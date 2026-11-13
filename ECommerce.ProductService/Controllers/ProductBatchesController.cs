using ECommerce.ProductService.DTOs;
using ECommerce.ProductService.Models;
using ECommerce.ProductService.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ProductService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductBatchesController : ControllerBase
    {
        private readonly IProductBatchService
            _productBatchService;

        private readonly ILogger<ProductBatchesController>
            _logger;

        public ProductBatchesController(
            IProductBatchService productBatchService,
            ILogger<ProductBatchesController> logger)
        {
            _productBatchService =
                productBatchService;

            _logger = logger;
        }

        // --------------------------------------------------
        // GET ALL BATCHES
        // GET: api/ProductBatches
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBatches()
        {
            _logger.LogInformation(
                "Getting all product batches.");

            var batches =
                await _productBatchService
                    .GetBatchesAsync();

            return Ok(batches);
        }

        // --------------------------------------------------
        // GET BATCH BY ID
        // GET: api/ProductBatches/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBatch(
            int id)
        {
            _logger.LogInformation(
                "Getting batch with ID {BatchId}.",
                id);

            var batch =
                await _productBatchService
                    .GetBatchByIdAsync(id);

            return Ok(batch);
        }

        // --------------------------------------------------
        // GET BATCHES BY PRODUCT
        // GET: api/ProductBatches/product/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpGet("product/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            GetBatchesByProduct(
                int productId)
        {
            _logger.LogInformation(
                "Getting batches for product {ProductId}.",
                productId);

            var batches =
                await _productBatchService
                    .GetBatchesByProductIdAsync(
                        productId);

            return Ok(batches);
        }

        // --------------------------------------------------
        // CREATE BATCH
        // POST: api/ProductBatches
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBatch(
            CreateProductBatchDto dto)
        {
            _logger.LogInformation(
                "Creating batch {BatchNumber} " +
                "for product {ProductId}.",
                dto.BatchNumber,
                dto.ProductId);

            var batch =
                new ProductBatch
                {
                    ProductId =
                        dto.ProductId,

                    BatchNumber =
                        dto.BatchNumber,

                    Quantity =
                        dto.Quantity,

                    ManufacturingDate =
                        dto.ManufacturingDate,

                    ExpiryDate =
                        dto.ExpiryDate,

                    IsActive =
                        dto.IsActive
                };

            var createdBatch =
                await _productBatchService
                    .AddBatchAsync(batch);

            _logger.LogInformation(
                "Batch created successfully " +
                "with ID {BatchId}.",
                createdBatch.Id);

            return CreatedAtAction(
                nameof(GetBatch),
                new
                {
                    id = createdBatch.Id
                },
                createdBatch);
        }

        // --------------------------------------------------
        // UPDATE BATCH
        // PUT: api/ProductBatches/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBatch(
            int id,
            UpdateProductBatchDto dto)
        {
            _logger.LogInformation(
                "Updating batch with ID {BatchId}.",
                id);

            var batch =
                new ProductBatch
                {
                    BatchNumber =
                        dto.BatchNumber,

                    Quantity =
                        dto.Quantity,

                    ManufacturingDate =
                        dto.ManufacturingDate,

                    ExpiryDate =
                        dto.ExpiryDate,

                    IsActive =
                        dto.IsActive
                };

            var updatedBatch =
                await _productBatchService
                    .UpdateBatchAsync(
                        id,
                        batch);

            _logger.LogInformation(
                "Batch with ID {BatchId} " +
                "updated successfully.",
                id);

            return Ok(updatedBatch);
        }

        // --------------------------------------------------
        // DELETE BATCH
        // DELETE: api/ProductBatches/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBatch(
            int id)
        {
            _logger.LogInformation(
                "Deleting batch with ID {BatchId}.",
                id);

            await _productBatchService
                .DeleteBatchAsync(id);

            _logger.LogInformation(
                "Batch with ID {BatchId} " +
                "deleted successfully.",
                id);

            return NoContent();
        }
    }
}