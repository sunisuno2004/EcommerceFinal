using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.UserService.Data
{
    public class UserDbContextFactory
        : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(
            string[] args)
        {
            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(
                        Directory.GetCurrentDirectory())
                    .AddJsonFile(
                        "appsettings.json",
                        optional: false)
                    .AddJsonFile(
                        "appsettings.Development.json",
                        optional: true)
                    .Build();

            var connectionString =
                configuration.GetConnectionString(
                    "UserDb");

            var optionsBuilder =
                new DbContextOptionsBuilder<UserDbContext>();

            optionsBuilder.UseSqlServer(
                connectionString);

            return new UserDbContext(
                optionsBuilder.Options);
        }
    }
}