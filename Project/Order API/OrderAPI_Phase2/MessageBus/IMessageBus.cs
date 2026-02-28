using OrderAPI_Phase2.Events;

namespace OrderAPI_Phase2.MessageBus
{
    /// Interface for publishing messages to RabbitMQ.
    /// 
    /// Same pattern you used in Phase 2 with IProductHttpClient:
    /// OrderService depends on this INTERFACE, not on RabbitMQ directly.
    /// This means:
    /// - Easy to swap RabbitMQ with Kafka later (just change implementation)
    /// - Easy to mock in unit tests
    /// - Clean separation of concerns
    public interface IMessageBus
    {
        Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent);
    }
}
