using ECommerce.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IInventoryService inventoryService,
            IProductService productService,
            ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _productService = productService;
            _logger = logger;
        }

        // --------------------------------------------------
        // GET ALL INVENTORY
        // GET: api/Inventory
        // ADMIN ONLY
        //
        // Product.StockQuantity is treated as a
        // synchronized/derived summary of batch inventory.
        // --------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            _logger.LogInformation(
                "Admin requested complete inventory.");

            var products =
                await _productService.GetProductsAsync();

            var inventory = products.Select(product => new
            {
                productId = product.Id,
                productName = product.Name,
                stockQuantity = product.StockQuantity
            });

            return Ok(inventory);
        }

        // --------------------------------------------------
        // GET AVAILABLE STOCK FOR ONE PRODUCT
        // GET: api/Inventory/1
        // ADMIN ONLY
        //
        // IMPORTANT:
        // Available stock is calculated from ProductBatch,
        // which is the source of truth for inventory.
        // --------------------------------------------------
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetStock(
            int productId)
        {
            var product =
                await _productService
                    .GetProductByIdAsync(productId);

            var availableStock =
                await _inventoryService
                    .GetAvailableStockAsync(productId);

            _logger.LogInformation(
                "Retrieved inventory for product {ProductId}. " +
                "Available stock: {AvailableStock}.",
                productId,
                availableStock);

            return Ok(new
            {
                productId = product.Id,
                productName = product.Name,
                stockQuantity = availableStock
            });
        }
    }
}