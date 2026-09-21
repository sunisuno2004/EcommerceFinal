using ECommerce.API.DTOs;
using ECommerce.Business.Exceptions;
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
        // --------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            _logger.LogInformation(
                "Getting inventory.");

            var products =
                await _productService.GetProductsAsync();

            var inventory = products.Select(p => new
            {
                productId = p.Id,
                productName = p.Name,
                stockQuantity = p.StockQuantity
            });

            return Ok(inventory);
        }

        // --------------------------------------------------
        // GET STOCK FOR ONE PRODUCT
        // GET: api/Inventory/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetStock(
            int productId)
        {
            var product =
                await _inventoryService.GetStockAsync(
                    productId);

            return Ok(new
            {
                productId = product.Id,
                productName = product.Name,
                stockQuantity = product.StockQuantity
            });
        }

        // --------------------------------------------------
        // SET STOCK QUANTITY
        // PUT: api/Inventory/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateStock(
            int productId,
            UpdateStockDto dto)
        {
            if (dto.StockQuantity < 0)
            {
                throw new ValidationException(
                    "Stock quantity cannot be negative.");
            }

            var product =
                await _inventoryService.GetStockAsync(
                    productId);

            product.StockQuantity =
                dto.StockQuantity;

            var updatedProduct =
                await _productService.UpdateProductAsync(
                    productId,
                    product);

            _logger.LogInformation(
                "Stock updated for product {ProductId}. " +
                "New stock: {StockQuantity}.",
                productId,
                dto.StockQuantity);

            return Ok(new
            {
                message = "Stock updated successfully.",
                productId = updatedProduct.Id,
                productName = updatedProduct.Name,
                stockQuantity = updatedProduct.StockQuantity
            });
        }

        // --------------------------------------------------
        // ADD STOCK
        // POST: api/Inventory/1/add
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpPost("{productId}/add")]
        public async Task<IActionResult> AddStock(
            int productId,
            UpdateStockDto dto)
        {
            var updatedProduct =
                await _inventoryService.AddStockAsync(
                    productId,
                    dto.StockQuantity);

            _logger.LogInformation(
                "Added {Quantity} stock to product {ProductId}.",
                dto.StockQuantity,
                productId);

            return Ok(new
            {
                message = "Stock added successfully.",
                productId = updatedProduct.Id,
                productName = updatedProduct.Name,
                stockQuantity = updatedProduct.StockQuantity
            });
        }

        // --------------------------------------------------
        // REMOVE STOCK
        // POST: api/Inventory/1/remove
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpPost("{productId}/remove")]
        public async Task<IActionResult> RemoveStock(
            int productId,
            UpdateStockDto dto)
        {
            var updatedProduct =
                await _inventoryService.RemoveStockAsync(
                    productId,
                    dto.StockQuantity);

            _logger.LogInformation(
                "Removed {Quantity} stock from product {ProductId}.",
                dto.StockQuantity,
                productId);

            return Ok(new
            {
                message = "Stock removed successfully.",
                productId = updatedProduct.Id,
                productName = updatedProduct.Name,
                stockQuantity = updatedProduct.StockQuantity
            });
        }
    }
}