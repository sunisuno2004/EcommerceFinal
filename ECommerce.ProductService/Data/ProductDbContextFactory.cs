using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.ProductService.Data
{
    public class ProductDbContextFactory
        : IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(
            string[] args)
        {
            var basePath =
                Directory.GetCurrentDirectory();

            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile(
                        "appsettings.json",
                        optional: false)
                    .AddJsonFile(
                        "appsettings.Development.json",
                        optional: true)
                    .Build();

            var connectionString =
                configuration.GetConnectionString(
                    "ProductDb");

            var optionsBuilder =
                new DbContextOptionsBuilder<ProductDbContext>();

            optionsBuilder.UseSqlServer(
                connectionString);

            return new ProductDbContext(
                optionsBuilder.Options);
        }
    }
}