using OrderAPI_Phase2.Events;

namespace OrderAPI_Phase2.KafkaProducer
{
    /// <summary>
    /// Interface for publishing events to Kafka.
    /// 
    /// Same pattern as IMessageBus for RabbitMQ:
    /// - OrderService depends on abstraction, not Kafka directly
    /// - Easy to mock in unit tests
    /// - Clean separation of concerns
    /// </summary>
    public interface IOrderKafkaProducer
    {
        Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent);
    }
}
