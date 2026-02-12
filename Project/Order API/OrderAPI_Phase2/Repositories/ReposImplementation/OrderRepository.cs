using System.Collections.Concurrent;
using OrderAPI_Phase2.Models;
using OrderAPI_Phase2.Repositories.InterfaceRepo;

namespace OrderAPI_Phase2.Repositories.ReposImplementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<int, OrderDetails> _orders;

        private int _nextId = 1;

        public OrderRepository() //this is a constructor which is called automatically when an object is created
            //this creates the object with default values
        {
            _orders = new ConcurrentDictionary<int, OrderDetails>();

            //Expla - when an OrderRepository object is created, set up an empty, thread safe in-memory storage to 
            //hold orders, indexed by an integer Id
            //That’s it.
            //No database. No files. Just memory.
        }


        public Task<IEnumerable<OrderDetails>> GetAllOrdersAsync()
        {
            return Task.FromResult(_orders.Values.AsEnumerable());
        }

        public Task<OrderDetails?> GetOrderByIdAsync(int id)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order);
        }

        public Task<OrderDetails> CreateOrderAsync(OrderDetails order)
        {
            order.Id = Interlocked.Increment(ref _nextId);
            order.OrderDate = DateTime.UtcNow;

            _orders.TryAdd(order.Id, order);

            return Task.FromResult(order);
        }

    }
}
