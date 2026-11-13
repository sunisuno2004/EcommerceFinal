using System.Net.Http.Json;

namespace ECommerce.OrderService.Clients
{
    public class ProductServiceClient
        : IProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductResponse?>
            GetProductAsync(
                int productId)
        {
            return await _httpClient
                .GetFromJsonAsync<ProductResponse>(
                    $"/api/Products/{productId}");
        }

        public async Task<int>
            GetAvailableStockAsync(
                int productId)
        {
            var response =
                await _httpClient
                    .GetFromJsonAsync<InventoryResponse>(
                        $"/api/Inventory/{productId}");

            return response?.StockQuantity ?? 0;
        }
        public async Task DeductStockAsync(
    int productId,
    int quantity)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    $"/api/Inventory/{productId}/deduct",
                    new
                    {
                        quantity
                    });

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new Exception(
                    $"Failed to deduct stock for product " +
                    $"{productId}. {error}");
            }
        }
        private class InventoryResponse
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }
                = string.Empty;

            public int StockQuantity { get; set; }
        }
    }
}