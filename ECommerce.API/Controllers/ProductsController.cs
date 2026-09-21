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

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
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
                StockQuantity = dto.StockQuantity
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
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
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
        // --------------------------------------------------
        [HttpPost("bulk-upload")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BulkUpload(
            IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                throw new ValidationException(
                    "Please upload an Excel file.");
            }

            var extension =
                Path.GetExtension(file.FileName);

            if (!string.Equals(
                    extension,
                    ".xlsx",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Only .xlsx Excel files are supported.");
            }

            var products = new List<Product>();
            var errors = new List<string>();

            using var stream =
                file.OpenReadStream();

            using var workbook =
                new XLWorkbook(stream);

            var worksheet =
                workbook.Worksheets.FirstOrDefault();

            if (worksheet is null)
            {
                throw new ValidationException(
                    "Excel file does not contain a worksheet.");
            }

            var lastRow =
                worksheet.LastRowUsed()?.RowNumber() ?? 0;

            if (lastRow < 2)
            {
                throw new ValidationException(
                    "Excel file does not contain product data.");
            }

            // Expected Excel columns:
            // Name | Description | Price | StockQuantity

            var headers =
                new Dictionary<string, int>(
                    StringComparer.OrdinalIgnoreCase);

            var headerRow =
                worksheet.Row(1);

            foreach (var cell in headerRow.CellsUsed())
            {
                var header =
                    cell.GetString().Trim();

                if (!string.IsNullOrWhiteSpace(header))
                {
                    headers[header] =
                        cell.Address.ColumnNumber;
                }
            }

            if (!headers.ContainsKey("Name") ||
                !headers.ContainsKey("Description") ||
                !headers.ContainsKey("Price") ||
                !headers.ContainsKey("StockQuantity"))
            {
                throw new ValidationException(
                    "Excel must contain these columns: " +
                    "Name, Description, Price, StockQuantity.");
            }

            for (
                int rowNumber = 2;
                rowNumber <= lastRow;
                rowNumber++)
            {
                var row =
                    worksheet.Row(rowNumber);

                var name =
                    row.Cell(headers["Name"])
                        .GetString()
                        .Trim();

                var description =
                    row.Cell(headers["Description"])
                        .GetString()
                        .Trim();

                var priceText =
                    row.Cell(headers["Price"])
                        .GetString()
                        .Trim();

                var stockText =
                    row.Cell(headers["StockQuantity"])
                        .GetString()
                        .Trim();

                // Ignore completely empty rows
                if (string.IsNullOrWhiteSpace(name) &&
                    string.IsNullOrWhiteSpace(priceText) &&
                    string.IsNullOrWhiteSpace(stockText))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add(
                        $"Row {rowNumber}: " +
                        "Product name is required.");

                    continue;
                }

                if (!decimal.TryParse(
                        priceText,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var price))
                {
                    errors.Add(
                        $"Row {rowNumber}: " +
                        $"Invalid price for '{name}'.");

                    continue;
                }

                if (price <= 0)
                {
                    errors.Add(
                        $"Row {rowNumber}: " +
                        "Price must be greater than zero.");

                    continue;
                }

                if (!int.TryParse(
                        stockText,
                        out var stockQuantity))
                {
                    errors.Add(
                        $"Row {rowNumber}: " +
                        $"Invalid stock quantity for '{name}'.");

                    continue;
                }

                if (stockQuantity < 0)
                {
                    errors.Add(
                        $"Row {rowNumber}: " +
                        "Stock quantity cannot be negative.");

                    continue;
                }

                products.Add(
                    new Product
                    {
                        Name = name,

                        Description =
                            string.IsNullOrWhiteSpace(description)
                                ? null
                                : description,

                        Price = price,

                        StockQuantity =
                            stockQuantity
                    });
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(
                    "Excel contains invalid data. " +
                    string.Join(" ", errors));
            }

            if (products.Count == 0)
            {
                throw new ValidationException(
                    "No valid products found in the Excel file.");
            }

            var addedProducts =
                await _productService.AddProductsAsync(products);

            _logger.LogInformation(
                "{Count} products uploaded successfully.",
                addedProducts.Count);

            return Ok(new
            {
                message =
                    "Products uploaded successfully.",

                totalProductsAdded =
                    addedProducts.Count,

                products =
                    addedProducts.Select(MapToProductDto)
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