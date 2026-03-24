using Microsoft.EntityFrameworkCore;
using ProductAPI_Phase1.Models;

namespace ProductAPI_Phase1.Data
{

    /// <summary>
    /// DbContext = Bridge between C# code and SQL Server database
    /// 
    /// Think of it as a warehouse manager:
    /// - Knows where all data is stored
    /// - Handles all database operations
    /// - Tracks changes to your objects
    /// - Translates LINQ queries → SQL
    /// </summary>
    /// 


    /// HasData adds seed data:
     ///   └── 3 products inserted ONCE
        ////    when database first created
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
          : base(options)
        {

        }

        // DbSet = represents a table in database
        // DbSet<Product> = Products table in SQL Server
        public DbSet<Product> Products_tbl { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product table
            modelBuilder.Entity<Product>(entity =>
            {
                // Table name in SQL Server
                entity.ToTable("Products_tbl");

                // Primary key
                entity.HasKey(e => e.Id);

                // Auto increment Id
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Required fields with max length
                entity.Property(e => e.Prod_Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                // Decimal precision for price
                // 18 = total digits, 2 = decimal places
                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.Property(e => e.CostPrice)
                    .HasPrecision(18, 2);

                // Optional fields
                entity.Property(e => e.ProdImageUrl)
                    .HasMaxLength(500);

                // Seed data — pre-populate Products table
                // Same data you had in InMemoryRepository
                entity.HasData(
                    new Product
                    {
                        Id = 1,
                        ProductId = 12,
                        Prod_Name = "Laptop Dell XPS 15",
                        Description = "High performance laptop with 16GB RAM and 512GB SSD",
                        Price = 1299.99m,
                        CostPrice = 950.00m,
                        Stock = 25,
                        Category = "Electronics",
                        ProdImageUrl = "https://example.com/laptop.jpg",
                        CreatedAt = new DateTime(2024, 1, 1),
                        SupplierId = 101
                    },
                    new Product
                    {
                        Id = 2,
                        ProductId = 15,
                        Prod_Name = "Wireless Mouse Logitech MX Master 3",
                        Description = "Ergonomic wireless mouse with precision scrolling",
                        Price = 99.99m,
                        CostPrice = 60.00m,
                        Stock = 150,
                        Category = "Electronics",
                        ProdImageUrl = "https://example.com/mouse.jpg",
                        CreatedAt = new DateTime(2024, 1, 1),
                        SupplierId = 102
                    },
                    new Product
                    {
                        Id = 3,
                        ProductId = 18,
                        Prod_Name = "Herman Miller Aeron Chair",
                        Description = "Premium ergonomic office chair with lumbar support",
                        Price = 1495.00m,
                        CostPrice = 890.00m,
                        Stock = 15,
                        Category = "Furniture",
                        ProdImageUrl = "https://example.com/chair.jpg",
                        CreatedAt = new DateTime(2024, 1, 1),
                        SupplierId = 103
                    }
                );
            });
        }
    }


}

