using OrderAPI_Phase2.DTOs;

namespace OrderAPI_Phase2.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDTO createOrderDto);
    }
}
