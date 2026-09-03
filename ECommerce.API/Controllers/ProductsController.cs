using ECommerce.API.DTOs;
using ECommerce.Business.Models;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("Getting all products.");

            var products = await _productService.GetProductsAsync();

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();

            _logger.LogInformation(
                "Retrieved {Count} products.",
                productDtos.Count);

            return Ok(productDtos);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            _logger.LogInformation(
                "Getting product with ID {ProductId}.",
                id);

            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning(
                    "Product with ID {ProductId} was not found.",
                    id);

                return NotFound();
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            return Ok(productDto);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(
     CreateProductDto dto)
        {
            _logger.LogInformation(
                "Creating product {ProductName}.",
                dto.Name);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            var addedProduct =
                await _productService.AddProductAsync(product);

            _logger.LogInformation(
                "Product created successfully with ID {ProductId}.",
                addedProduct.Id);

            var productDto = new ProductDto
            {
                Id = addedProduct.Id,
                Name = addedProduct.Name,
                Description = addedProduct.Description,
                Price = addedProduct.Price,
                StockQuantity = addedProduct.StockQuantity
            };

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = addedProduct.Id },
                productDto);
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            UpdateProductDto dto)
        {
            _logger.LogInformation(
                "Updating product with ID {ProductId}.",
                id);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            var updatedProduct =
                await _productService.UpdateProductAsync(id, product);

            if (updatedProduct == null)
            {
                _logger.LogWarning(
                    "Product with ID {ProductId} was not found for update.",
                    id);

                return NotFound();
            }

            _logger.LogInformation(
                "Product with ID {ProductId} updated successfully.",
                id);

            var productDto = new ProductDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                StockQuantity = updatedProduct.StockQuantity
            };

            return Ok(productDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation(
                "Deleting product with ID {ProductId}.",
                id);

            var deleted =
                await _productService.DeleteProductAsync(id);

            if (!deleted)
            {
                _logger.LogWarning(
                    "Product with ID {ProductId} was not found for deletion.",
                    id);

                return NotFound();
            }

            _logger.LogInformation(
                "Product with ID {ProductId} deleted successfully.",
                id);

            return NoContent();
        }
    }
}