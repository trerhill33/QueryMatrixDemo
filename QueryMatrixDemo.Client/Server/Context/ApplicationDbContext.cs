using Microsoft.EntityFrameworkCore;
using QueryMatrixDemo.Shared.Entities;

namespace QueryMatrixDemo.Client.Server.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        // Order configuration
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);

        // InventoryItem configuration
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Product)
            .WithMany()
            .HasForeignKey(ii => ii.ProductId);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Warehouse)
            .WithMany(w => w.InventoryItems)
            .HasForeignKey(ii => ii.WarehouseId);

        // Add some seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Clothing" },
            new Category { Id = 3, Name = "Books" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Description = "High-end laptop", Price = 999.99m, CategoryId = 1, InventoryQuantity = 50 },
            new Product { Id = 2, Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, CategoryId = 2, InventoryQuantity = 100 },
            new Product { Id = 3, Name = "Programming Book", Description = "C# Programming", Price = 49.99m, CategoryId = 3, InventoryQuantity = 30 }
        );

        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        );

        // Seed Warehouses
        modelBuilder.Entity<Warehouse>().HasData(
            new Warehouse { Id = 1, Name = "Main Warehouse", Location = "New York" },
            new Warehouse { Id = 2, Name = "Secondary Warehouse", Location = "Los Angeles" }
        );

        // Seed InventoryItems
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, ProductId = 1, WarehouseId = 1, Quantity = 30 },
            new InventoryItem { Id = 2, ProductId = 1, WarehouseId = 2, Quantity = 20 },
            new InventoryItem { Id = 3, ProductId = 2, WarehouseId = 1, Quantity = 50 }
        );
    }
}