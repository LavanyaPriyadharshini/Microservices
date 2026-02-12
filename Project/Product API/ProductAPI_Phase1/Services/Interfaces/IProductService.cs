using ProductAPI_Phase1.DTOs;

namespace ProductAPI_Phase1.Services.Interfaces
{
    public interface IProductService
        //these are the business logic methods
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);

        Task<ProductDto?> GetProductByProdIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDTO createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, CreateProductDTO updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
    }
}
