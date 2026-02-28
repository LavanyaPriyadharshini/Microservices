namespace ProductAPI_Phase1.DTOs
{
    public record ProductDto(
       int Id,
       int ProductId,
       string Prod_Name,
       string Description,
       decimal Price,
       int Stock,
       string Category,
       string? ImageUrl
   );
}
