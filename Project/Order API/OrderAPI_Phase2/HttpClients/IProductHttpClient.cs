using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Diagnostics;
using System;
using OrderAPI_Phase2.DTOs;

namespace OrderAPI_Phase2.HttpClients
{
    /// <summary>
    /// Interface for calling Product.API, as we are using synchronous operation we go for 
    /// </summary>
    public interface IProductHttpClient
    {
        Task<ProductDTO> GetProductByIdAsync(int id);

        Task<ProductDTO> GetProductByProdIdsAsync(int prodId);

        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();


    }


//    Key Points in HttpClient:

//Receives HttpClient via constructor - IHttpClientFactory provides it
//Base URL configured elsewhere - Not hardcoded here
//JsonSerializerOptions - Handles case sensitivity
//Error handling - Catches network failures
//Logging - Tracks every call for debugging
}
