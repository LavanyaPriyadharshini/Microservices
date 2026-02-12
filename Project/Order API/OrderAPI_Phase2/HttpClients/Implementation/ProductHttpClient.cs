using System.Text.Json;
using OrderAPI_Phase2.DTOs;

namespace OrderAPI_Phase2.HttpClients.Implementation
{
    public class ProductHttpClient : IProductHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductHttpClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductHttpClient(
            HttpClient httpClient,
            ILogger<ProductHttpClient> logger)

        {
            _httpClient = httpClient;
            _logger = logger;

            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,// Handle "Id" vs "id"
            };

        }


        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Calling Product API to get the product for the id");

                //make the HTTP GEt request to product.api
                //these are the endpoints from the Product api - phase 1
               // http://localhost:5263/api/Product/getallproducts - Product get all method
                //http://localhost:5263/api/Product/GetproductbyID?id=1

                var response = await _httpClient.GetAsync($"api/Product/GetproductbyID?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductDTO>(content, _jsonOptions);

                    _logger.LogInformation("Successfully retrieved product {ProductId} from Product.API", id);
                    return product;
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product {ProductId} not found in Product.API", id);
                    return null;
                }

                else
                {
                    _logger.LogError("Error calling Product.API. Status: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Product.API returned status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling Product.API for product {ProductId}", id);
                throw new Exception("Product service is unavailable. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Product.API for product {ProductId}", id);
                throw;
            }
        }



        public async Task<ProductDTO?> GetProductByProdIdsAsync(int prodId)
        {
            try
            {
                _logger.LogInformation("Calling Product API to get the product for the id");

                //make the HTTP GEt request to product.api
                //http://localhost:5263/api/Product/getallproducts - Product get all method
                //http://localhost:5263/api/Product/GetproductbyID?id=1

                var response = await _httpClient.GetAsync($"api/Product/GetproductbyProdID?prodId={prodId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductDTO>(content, _jsonOptions);

                    _logger.LogInformation("Successfully retrieved product {ProductId} from Product.API", prodId);
                    return product;
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product {ProductId} not found in Product.API", prodId);
                    return null;
                }

                else
                {
                    _logger.LogError("Error calling Product.API. Status: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Product.API returned status code: {response.StatusCode}");
                }

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling Product.API for product {ProductId}", prodId);
                throw new Exception("Product service is unavailable. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Product.API for product {ProductId}", prodId);
                throw;
            }
        }


        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Calling Product.API to get all products");

                var response = await _httpClient.GetAsync("api/Product/getallproducts");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(content, _jsonOptions);

                    _logger.LogInformation("Successfully retrieved products from Product.API");
                    return products ?? Enumerable.Empty<ProductDTO>();
                }
                else
                {
                    _logger.LogError("Error calling Product.API. Status: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"Product.API returned status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when calling Product.API");
                throw new Exception("Product service is unavailable. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Product.API");
                throw;
            }

        }
        }
}