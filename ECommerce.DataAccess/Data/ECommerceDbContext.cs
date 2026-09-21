using ECommerce.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(
            DbContextOptions<ECommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<ProductBatch> ProductBatches { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderItem>()
    .HasOne(o => o.Order)
    .WithMany(o => o.OrderItems)
    .HasForeignKey(o => o.OrderId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProductBatch>()
    .HasOne(b => b.Product)
    .WithMany()
    .HasForeignKey(b => b.ProductId)
    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductBatch>()
    .HasIndex(b => new { b.ProductId, b.BatchNumber })
    .IsUnique();
        }
    }
}