using ProductAPI_Phase1.DTOs;
using ProductAPI_Phase1.Models;
using ProductAPI_Phase1.Repositories;
using ProductAPI_Phase1.Services.Interfaces;

namespace ProductAPI_Phase1.Services.ServiceImplementation
{
    public class ProductService(IProductRepository repository,
        ILogger<ProductService> logger) : IProductService
    {

        ///static means the method belongs to the class itself, not to any specific instance of the class.
        ///// // ❌ Doesn't access _repository
        // ❌ Doesn't access _logger
        // ❌ Doesn't access any class fields
        // ✅ Only uses the parameter 'product'
        private static ProductDto MapToDto(Product product) => new(
 product.Id,
 product.ProductId,
 product.Prod_Name,
 product.Description,
 product.Price,
 product.Stock,
 product.Category,
 product.ProdImageUrl
);


        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            logger.LogInformation("Fetching all the products");

            var products = await repository.GetAllProductAsync(); //here if you have db ,you can accessusing the Generic repository and unit of work concept

            var productDTO = products.Select(MapToDto);

            logger.LogInformation("Retrieved {count} products",productDTO.Count());

            return productDTO;

        }





        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            logger.LogInformation("Fetching product with ID: {ProductId}", id);

            var product = await repository.GetProductByIdAsync(id);

            if (product is null)
            {
                logger.LogWarning("Product with ID {ProductId} not found", id);
                return null;
            }

            return MapToDto(product);
        }

        public async Task<ProductDto?> GetProductByProdIdAsync(int Prodid)
        {
            logger.LogInformation("Fetching product with ID: {ProductId}", Prodid);

            var product = await repository.GetProductByIdAsync(Prodid);

            if (product is null)
            {
                logger.LogWarning("Product with ID {ProductId} not found", Prodid);
                return null;
            }

            return MapToDto(product);
        }



        public async Task<ProductDto> CreateProductAsync(CreateProductDTO createProductDto)
        {
            logger.LogInformation("Creating new product: {ProductName}", createProductDto.Name);

            var product = new Product
            {
                Prod_Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                CostPrice = createProductDto.CostPrice,
                Stock = createProductDto.Stock,
                Category = createProductDto.Category,
                ProdImageUrl = createProductDto.ImageUrl,
                SupplierId = createProductDto.SupplierId
            };

            var createdProduct = await repository.CreateProductAsync(product);

            logger.LogInformation("Product created with ID: {ProductId}", createdProduct.Id);

            return MapToDto(createdProduct);
        }


        public async Task<ProductDto?> UpdateProductAsync(int id, CreateProductDTO updateProductDto)
        {
            logger.LogInformation("Updating product with ID: {ProductId}", id);

            var exists = await repository.ProductExistsAsync(id);
            if (!exists)
            {
                logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return null;
            }

            var product = new Product
            {
                Id = id,
                Prod_Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                Price = updateProductDto.Price,
                CostPrice = updateProductDto.CostPrice,
                Stock = updateProductDto.Stock,
                Category = updateProductDto.Category,
                ProdImageUrl = updateProductDto.ImageUrl,
                SupplierId = updateProductDto.SupplierId
            };

            var updatedProduct = await repository.UpdateProductAsync(product);

            logger.LogInformation("Product {ProductId} updated successfully", id);

            return updatedProduct is not null ? MapToDto(updatedProduct) : null;
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            logger.LogInformation("Deleting product with ID: {ProductId}", id);

            var result = await repository.DeleteProductAsync(id);

            if (result)
                logger.LogInformation("Product {ProductId} deleted successfully", id);
            else
                logger.LogWarning("Product {ProductId} not found for deletion", id);

            return result;
        }


        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            logger.LogInformation("Fetching products in category: {Category}", category);

            var products = await repository.GetProductsByCategoryAsync(category);

            return products.Select(MapToDto);
        }

   

    }


}
