using Microsoft.EntityFrameworkCore;
using ProductAPI_Phase1.Repositories;


namespace ProductAPI_Phase1.Repositories_Implementation
{
    /// <summary>
    /// Generic EF Core implementation of IGenericRepository.
    /// 
    /// TContext = your specific DbContext type
    /// T        = your entity/model class
    /// 
    /// Example:
    /// EFGenericRepository<ProductDbContext, Product>
    /// → uses ProductDbContext
    /// → works with Product entity
    /// → maps to Products table
    /// </summary>

    public class EFGenericRepository<TContext, T>
         : IGenericRepository<T>
         where TContext : DbContext
         where T : class
    {

        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public EFGenericRepository(TContext context)
        {
            _context = context;

            // context.Set<T>() = gets the correct DbSet for T
            // If T = Product → returns _context.Products
            // If T = Order   → returns _context.Orders
            // EF Core figures out the correct table automatically ✅
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // SELECT * FROM [TableName]
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            // SELECT * FROM [TableName] WHERE Id = @id
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            // Prepare INSERT statement
            await _dbSet.AddAsync(entity);

            // Execute INSERT INTO [TableName] VALUES (...)
            await _context.SaveChangesAsync();

            return entity;
        }

        //public async Task<T?> UpdateAsync(T entity)
        //{
        //    // Mark entity as modified
        //    // EF Core tracks what changed
        //    _dbSet.Update(entity);

        //    // Execute UPDATE [TableName] SET ... WHERE Id = @id
        //    await _context.SaveChangesAsync();

        //    return entity;
        //}

        public async Task<T?> UpdateAsync(T entity)
        {
            // Update already tracked entity
            // Since we fetched it via FindAsync it's already tracked
            // SaveChangesAsync will detect changes automatically
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity is null) return false;

            // Mark for deletion
            _dbSet.Remove(entity);

            // Execute DELETE FROM [TableName] WHERE Id = @id
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            // Most efficient existence check
            // SELECT CASE WHEN EXISTS(SELECT 1 FROM [TableName] WHERE Id=@id)
            var entity = await _dbSet.FindAsync(id);
            return entity is not null;
        }
    }
}