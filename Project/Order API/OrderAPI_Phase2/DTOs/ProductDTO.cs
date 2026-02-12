namespace OrderAPI_Phase2.DTOs
{

    /// <summary>
    /// DTO to receive product data from Product.API
    /// This matches the ProductDto from Product.API
    /// the properties here should match exactly as in the product model of the Product API project
    /// </summary>
    public record ProductDTO
    (
        int Id,
        int ProductId,
    string Prod_Name,
    string Description,
    decimal Price,
    int Stock,
    string Category,
    string? ProdImageUrl
    );
}
