using ECommerce.ProductService.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService
            _inventoryService;

        private readonly IProductService
            _productService;

        private readonly ILogger<InventoryController>
            _logger;
        public class DeductStockRequest
        {
            public int Quantity { get; set; }
        }
        public InventoryController(
            IInventoryService inventoryService,
            IProductService productService,
            ILogger<InventoryController> logger)
        {
            _inventoryService =
                inventoryService;

            _productService =
                productService;

            _logger =
                logger;
        }

        // --------------------------------------------------
        // GET ALL INVENTORY
        // GET: api/Inventory
        // ADMIN ONLY
        //
        // Product.StockQuantity is a synchronized
        // summary of batch inventory.
        // --------------------------------------------------
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInventory()
        {
            _logger.LogInformation(
                "Getting complete inventory.");

            var products =
                await _productService
                    .GetProductsAsync();

            var inventory =
                products.Select(product => new
                {
                    productId =
                        product.Id,

                    productName =
                        product.Name,

                    stockQuantity =
                        product.StockQuantity
                });

            return Ok(inventory);
        }
        [Authorize]
        [HttpPost("{productId}/deduct")]
        public async Task<IActionResult> DeductStock(
    int productId,
    [FromBody] DeductStockRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(
                    "Quantity must be greater than zero.");
            }

            await _inventoryService.DeductStockAsync(
                productId,
                request.Quantity);

            return Ok(new
            {
                message = "Stock deducted successfully."
            });
        }
        // --------------------------------------------------
        // GET STOCK FOR ONE PRODUCT
        // GET: api/Inventory/1
        // ADMIN ONLY
        //
        // Available stock is calculated from ProductBatch.
        // --------------------------------------------------
        [HttpGet("{productId:int}")]
        [Authorize]
        public async Task<IActionResult> GetStock(
            int productId)
        {
            var product =
                await _productService
                    .GetProductByIdAsync(
                        productId);

            var availableStock =
                await _inventoryService
                    .GetAvailableStockAsync(
                        productId);

            _logger.LogInformation(
                "Retrieved inventory for product {ProductId}. " +
                "Available stock: {AvailableStock}.",
                productId,
                availableStock);

            return Ok(
                new
                {
                    productId =
                        product.Id,

                    productName =
                        product.Name,

                    stockQuantity =
                        availableStock
                });
        }
    }
}