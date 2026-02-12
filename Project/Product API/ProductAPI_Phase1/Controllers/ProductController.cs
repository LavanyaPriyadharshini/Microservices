using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI_Phase1.DTOs;
using ProductAPI_Phase1.Services.Interfaces;

namespace ProductAPI_Phase1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class ProductController(
        IProductService productService,
        ILogger<ProductController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all products
        /// </summary>

        [HttpGet("getallproducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            logger.LogInformation("API: getting all products");
            var products = await productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("GetproductbyID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            logger.LogInformation("API: Getting product with ID: {productID}", id);

            var product = await productService.GetProductByIdAsync(id);

            return product is null ? NotFound(new { message = $"product with ID {id} not found" }) : Ok(product);
        }



        [HttpGet("GetproductbyProdID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProductByProductId(int prodId)
        {
            logger.LogInformation("API: Getting product with ID: {productID}", prodId);

            var product = await productService.GetProductByProdIdAsync(prodId);

            return product is null ? NotFound(new { message = $"product with ID {prodId} not found" }) : Ok(product);
        }


        /// <summary>
        /// Get products by category
        /// </summary>
        [HttpGet("category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category)
        {
            logger.LogInformation("API: Getting products in category: {Category}", category);
            var products = await productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }



        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDTO createProductDto)
        {
            logger.LogInformation("API: Creating new product: {ProductName}", createProductDto.Name);

            var product = await productService.CreateProductAsync(createProductDto);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product
            );
        }


        /// <summary>
        /// Update existing product
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, CreateProductDTO updateProductDto)
        {
            logger.LogInformation("API: Updating product with ID: {ProductId}", id);

            var product = await productService.UpdateProductAsync(id, updateProductDto);

            return product is null
                ? NotFound(new { message = $"Product with ID {id} not found" })
                : Ok(product);
        }



        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            logger.LogInformation("API: Deleting product with ID: {ProductId}", id);

            var result = await productService.DeleteProductAsync(id);

            return result
                ? NoContent()
                : NotFound(new { message = $"Product with ID {id} not found" });
        }

    }
}
