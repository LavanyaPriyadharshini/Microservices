using OrderAPI_Phase2.Models;

namespace OrderAPI_Phase2.Repositories.InterfaceRepo
{

    /// <summary>
    /// Contract for order data access
    /// </summary>
    /// 

    public interface IOrderRepository
    {
        //OrderDetails is the entity or the model name
        Task<IEnumerable<OrderDetails>> GetAllOrdersAsync();
        Task<OrderDetails?> GetOrderByIdAsync(int id);
        Task<OrderDetails> CreateOrderAsync(OrderDetails order);
    }
}
