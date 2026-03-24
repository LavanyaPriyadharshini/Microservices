using ProductAPI_Phase1.Data;
using ProductAPI_Phase1.Models;
using Microsoft.EntityFrameworkCore;  // ← needed for ToListAsync, FirstOrDefaultAsync

using ProductAPI_Phase1.Repositories;

namespace ProductAPI_Phase1.Repositories_Implementation
{
    /// <summary>
    /// Product-specific EF Core repository.
    /// 
    /// Extends EFGenericRepository which gives us all CRUD for free.
    /// We only implement Product-specific methods here.
    /// 
    /// Inheritance chain:
    /// EFProductRepository
    ///     → EFGenericRepository<ProductDbContext, Product>  (CRUD)
    ///         → IGenericRepository<Product>                 (interface)
    ///     → IProductRepository                              (product specific)
    /// </summary>
    public class EFProductRepository
        : EFGenericRepository<ProductDbContext, Product>,
          IProductRepository

    {
        private readonly ILogger<EFProductRepository> _logger;

        public EFProductRepository(
               ProductDbContext context,
               ILogger<EFProductRepository> logger)
               : base(context)  // pass context to EFGenericRepository
        {
            _logger = logger;
        }

        // ── Product-specific methods only ──────────────────────────

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            _logger.LogInformation("Fetching products in category: {Category}", category);

            return await _context.Products_tbl
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<Product?> GetProductByProdIdAsync(int prodId)
        {
            _logger.LogInformation("Fetching product by ProductId: {ProdId}", prodId);

            return await _context.Products_tbl
                .FirstOrDefaultAsync(p => p.ProductId == prodId);
        }
    }
}
