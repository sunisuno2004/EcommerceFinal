using ClosedXML.Excel;
using ECommerce.API.DTOs;
using ECommerce.Business.Exceptions;
using ECommerce.Business.Models;
using ECommerce.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IBulkProductImportService _bulkProductImportService;

        public ProductsController(
     IProductService productService,
     IBulkProductImportService bulkProductImportService,
     ILogger<ProductsController> logger)
        {
            _productService = productService;
            _bulkProductImportService =
                bulkProductImportService;
            _logger = logger;
        }

        // --------------------------------------------------
        // GET ALL PRODUCTS
        // GET: api/Products
        // --------------------------------------------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation(
                "Getting all products.");

            var products =
                await _productService.GetProductsAsync();

            var productDtos = products
                .Select(MapToProductDto)
                .ToList();

            _logger.LogInformation(
                "Retrieved {Count} products.",
                productDtos.Count);

            return Ok(productDtos);
        }

        // --------------------------------------------------
        // GET PRODUCT BY ID
        // GET: api/Products/1
        // --------------------------------------------------
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            _logger.LogInformation(
                "Getting product with ID {ProductId}.",
                id);

            var product =
                await _productService.GetProductByIdAsync(id);

            var productDto =
                MapToProductDto(product);

            return Ok(productDto);
        }

        // --------------------------------------------------
        // ADD PRODUCT
        // POST: api/Products
        // ADMIN ONLY
        // --------------------------------------------------
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
                StockQuantity = 0
            };

            var addedProduct =
                await _productService.AddProductAsync(product);

            _logger.LogInformation(
                "Product created successfully with ID {ProductId}.",
                addedProduct.Id);

            var productDto =
                MapToProductDto(addedProduct);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = addedProduct.Id },
                productDto);
        }

        // --------------------------------------------------
        // UPDATE PRODUCT
        // PUT: api/Products/1
        // ADMIN ONLY
        // --------------------------------------------------
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
                Price = dto.Price
            };

            var updatedProduct =
                await _productService.UpdateProductAsync(
                    id,
                    product);

            _logger.LogInformation(
                "Product with ID {ProductId} updated successfully.",
                id);

            var productDto =
                MapToProductDto(updatedProduct);

            return Ok(productDto);
        }

        // --------------------------------------------------
        // DELETE PRODUCT
        // DELETE: api/Products/1
        // ADMIN ONLY
        // --------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation(
                "Deleting product with ID {ProductId}.",
                id);

            await _productService.DeleteProductAsync(id);

            _logger.LogInformation(
                "Product with ID {ProductId} deleted successfully.",
                id);

            return NoContent();
        }

        // --------------------------------------------------
        // BULK PRODUCT UPLOAD
        // POST: api/Products/bulk-upload
        // ADMIN ONLY
        //
        // IMPORTANT:
        // Bulk upload creates product master data only.
        // Inventory must be created using ProductBatch.
        // --------------------------------------------------
        [HttpPost("bulk-upload")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BulkUpload(
           IFormFile file)
        {
            if (file is null ||
                file.Length == 0)
            {
                throw new ValidationException(
                    "Please upload an Excel file.");
            }

            var extension =
                Path.GetExtension(
                    file.FileName);

            if (!string.Equals(
                    extension,
                    ".xlsx",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Only .xlsx Excel files are supported.");
            }

            using var stream =
                file.OpenReadStream();

            var addedProducts =
                await _bulkProductImportService
                    .ImportAsync(stream);

            _logger.LogInformation(
                "{Count} products uploaded successfully.",
                addedProducts.Count);

            return Ok(new
            {
                message =
                    "Products uploaded successfully. " +
                    "Inventory must be added using product batches.",

                totalProductsAdded =
                    addedProducts.Count,

                products =
                    addedProducts.Select(
                        MapToProductDto)
            });
        }

        // --------------------------------------------------
        // PRODUCT -> PRODUCT DTO
        // --------------------------------------------------
        private static ProductDto MapToProductDto(
            Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }
    }
}