using ECommerce.API.DTOs;
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
        private readonly IProductService _productService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IProductService productService,
            ILogger<InventoryController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            var products =
                await _productService.GetProductsAsync();

            return Ok(products.Select(p => new
            {
                productId = p.Id,
                productName = p.Name,
                stockQuantity = p.StockQuantity
            }));
        }

        // PUT: api/inventory/1
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateStock(
            int productId,
            UpdateStockDto dto)
        {
            var product =
                await _productService.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            product.StockQuantity = dto.StockQuantity;

            var updatedProduct =
                await _productService.UpdateProductAsync(
                    productId,
                    product);

            _logger.LogInformation(
                "Stock updated for product {ProductId}. New stock: {Stock}",
                productId,
                dto.StockQuantity);

            return Ok(new
            {
                message = "Stock updated successfully.",
                productId = updatedProduct!.Id,
                productName = updatedProduct.Name,
                stockQuantity = updatedProduct.StockQuantity
            });
        }
    }
}