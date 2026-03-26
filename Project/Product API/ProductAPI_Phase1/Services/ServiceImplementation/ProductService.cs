using Microsoft.Extensions.Logging;
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

            //this is for the satic reposiory , now we have created a new genric repository with which we have connected the database
            //var products = await repository.GetAllProductAsync(); //here if you have db ,you can accessusing the Generic repository and unit of work concept

            var products = await repository.GetAllAsync();

            var productDTO = products.Select(MapToDto);

            logger.LogInformation("Retrieved {count} products",productDTO.Count());

            return productDTO;

        }





        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            logger.LogInformation("Fetching product with ID: {ProductId}", id);

          //  var product = await repository.GetProductByIdAsync(id);


            var product = await repository.GetByIdAsync(id);


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

            // var product = await repository.GetProductByIdAsync(Prodid);

            var product = await repository.GetByIdAsync(Prodid);


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
                ProductId=createProductDto.ProductId,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                CostPrice = createProductDto.CostPrice,
                Stock = createProductDto.Stock,
                Category = createProductDto.Category,
                ProdImageUrl = createProductDto.ImageUrl,
                SupplierId = createProductDto.SupplierId
            };

            //var createdProduct = await repository.CreateProductAsync(product);

            var createdProduct = await repository.CreateAsync(product);


            logger.LogInformation("Product created with ID: {ProductId}", createdProduct.Id);

            return MapToDto(createdProduct);
        }


        public async Task<ProductDto?> UpdateProductAsync(int id, CreateProductDTO updateProductDto)
        {
            logger.LogInformation("Updating product with ID: {ProductId}", id);

            var existingProduct = await repository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return null;
            }

            // Update existing tracked entity properties
            existingProduct.Prod_Name = updateProductDto.Name;
            existingProduct.Description = updateProductDto.Description;
            existingProduct.Price = updateProductDto.Price;
            existingProduct.CostPrice = updateProductDto.CostPrice;
            existingProduct.Stock = updateProductDto.Stock;
            existingProduct.Category = updateProductDto.Category;
            existingProduct.ProdImageUrl = updateProductDto.ImageUrl;
            existingProduct.SupplierId = updateProductDto.SupplierId;

            // ✅ FIXED — pass entity to UpdateAsync
            await repository.UpdateAsync(existingProduct);

            logger.LogInformation("Product {ProductId} updated successfully", id);

            return MapToDto(existingProduct);
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            logger.LogInformation("Deleting product with ID: {ProductId}", id);

           // var result = await repository.DeleteProductAsync(id);

            var result = await repository.DeleteAsync(id);

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
