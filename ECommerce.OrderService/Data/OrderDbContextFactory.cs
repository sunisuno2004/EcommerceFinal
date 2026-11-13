using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.OrderService.Data
{
    public class OrderDbContextFactory
        : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(
            string[] args)
        {
            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(
                        Directory.GetCurrentDirectory())
                    .AddJsonFile(
                        "appsettings.json")
                    .Build();

            var optionsBuilder =
                new DbContextOptionsBuilder<
                    OrderDbContext>();

            optionsBuilder.UseSqlServer(
                configuration
                    .GetConnectionString(
                        "OrderDb"));

            return new OrderDbContext(
                optionsBuilder.Options);
        }
    }
}