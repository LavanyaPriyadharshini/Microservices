using ProductAPI_Phase1.Models;

namespace ProductAPI_Phase1.Repositories
{
    public interface IProductRepository
    {
        // All methods return Task for consistency
        Task<IEnumerable<Product>> GetAllProductAsync();

        Task<Product?> GetProductByIdAsync(int id);
        //product? means Product or null

        Task<Product?> GetProductByProdIdAsync(int id);

        Task<Product> CreateProductAsync(Product product);
        //product , Never null(by contract)

        Task<Product?> UpdateProductAsync(Product product);

        Task<bool> DeleteProductAsync(int id);

        Task<bool> ProductExistsAsync(int id);

        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    }
}
