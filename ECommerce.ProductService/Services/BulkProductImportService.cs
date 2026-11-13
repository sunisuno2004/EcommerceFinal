using ClosedXML.Excel;
using ECommerce.ProductService.Exceptions;
using ECommerce.ProductService.Models;

using System.Globalization;

namespace ECommerce.ProductService.Services
{
    public class BulkProductImportService
        : IBulkProductImportService
    {
        private readonly IProductService _productService;

        public BulkProductImportService(
            IProductService productService)
        {
            _productService = productService;
        }

        public async Task<List<Product>> ImportAsync(
            Stream excelStream)
        {
            var products = new List<Product>();
            var errors = new List<string>();

            using var workbook =
                new XLWorkbook(excelStream);

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
                !headers.ContainsKey("Price"))
            {
                throw new ValidationException(
                    "Excel must contain these columns: " +
                    "Name, Description, Price.");
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

                if (string.IsNullOrWhiteSpace(name) &&
                    string.IsNullOrWhiteSpace(description) &&
                    string.IsNullOrWhiteSpace(priceText))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add(
                        $"Row {rowNumber}: Product name is required.");

                    continue;
                }

                if (!decimal.TryParse(
                        priceText,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var price))
                {
                    errors.Add(
                        $"Row {rowNumber}: Invalid price for '{name}'.");

                    continue;
                }

                if (price <= 0)
                {
                    errors.Add(
                        $"Row {rowNumber}: Price must be greater than zero.");

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
                        StockQuantity = 0
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

            return await _productService
                .AddProductsAsync(products);
        }
    }
}