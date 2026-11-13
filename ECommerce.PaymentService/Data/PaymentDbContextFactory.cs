using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.PaymentService.Data
{
    public class PaymentDbContextFactory
        : IDesignTimeDbContextFactory<
            PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(
            string[] args)
        {
            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(
                        Directory.GetCurrentDirectory())
                    .AddJsonFile(
                        "appsettings.json")
                    .Build();

            var options =
                new DbContextOptionsBuilder<
                    PaymentDbContext>();

            options.UseSqlServer(
                configuration
                    .GetConnectionString(
                        "PaymentDb"));

            return new PaymentDbContext(
                options.Options);
        }
    }
}