using ProductAPI_Phase1.Models;

namespace ProductAPI_Phase1.Repositories
{
    /// <summary>
    /// Generic Repository Interface — works for ANY entity T
    /// Here T represents the Model Class
    /// that is it works for any model class
    /// T = type parameter (like a placeholder) - it stands for Type
    /// where T : class = T must be a class (not int, string etc)
    /// 
    /// 
    /// 
    /// T = Product class
//       ↓
//     Maps to Products TABLE in SQL Server

//        T = Order class
//        ↓
//      Maps to Orders TABLE in SQL Server


    /// 
    /// Examples:
    /// IGenericRepository<Product> → for Product entity
    /// IGenericRepository<Order>   → for Order entity
    /// IGenericRepository<Invoice> → for Invoice entity
    /// 
    /// Common CRUD operations defined ONCE here
    /// Used by ALL repositories ✅
    /// </summary>
    /// 
    public interface IGenericRepository<T> where T : class
    {  
        // Get all records → SELECT * FROM table
        Task<IEnumerable<T>> GetAllAsync();

        // Get by ID → SELECT * FROM table WHERE Id = @id
        Task<T?> GetByIdAsync(int id);

        // Create → INSERT INTO table VALUES (...)
        Task<T> CreateAsync(T entity);

        // Update → UPDATE table SET ... WHERE Id = @id
        Task<T?> UpdateAsync(T entity);

        // Delete → DELETE FROM table WHERE Id = @id
        Task<bool> DeleteAsync(int id);

        // Check exists → SELECT CASE WHEN EXISTS(...) THEN 1 ELSE 0
        Task<bool> ExistsAsync(int id);

    }
}
