namespace OrderAPI_Phase2.DTOs
{
    /// <summary>
    /// DTO for order response
    /// </summary>
    public record OrderDto(
        int Id,
        int orderId,
        string CustomerName,
        string CustomerEmail,
        int ProductId,
        string ProductName,
        decimal ProductPrice,
        int Quantity,
        decimal TotalAmount,
        string Status,
        DateTime OrderDate
    );


}
