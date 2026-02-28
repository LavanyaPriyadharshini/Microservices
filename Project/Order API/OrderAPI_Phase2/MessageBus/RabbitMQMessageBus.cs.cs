using OrderAPI_Phase2.Events;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace OrderAPI_Phase2.MessageBus
{
    /// <summary>
    /// Concrete RabbitMQ implementation of IMessageBus.
    /// Registered as SINGLETON in DI because:
    /// - RabbitMQ connection is expensive to create (TCP handshake)
    /// - One shared connection for entire app lifetime is correct approach
    /// - IDisposable ensures connection is cleaned up on app shutdown
    /// </summary>
    public class RabbitMQMessageBus : IMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<RabbitMQMessageBus> _logger;

        // These names must match EXACTLY in Notification.API consumer
        // Exchange receives the message, routes it to the correct queue
        private const string ExchangeName = "order.exchange";
        private const string QueueName = "order.created.queue";
        private const string RoutingKey = "order.created"; //using this key only the messages gets exchanged



        //Explanation 1 : refer word file (microservice introduction)

        public RabbitMQMessageBus(
          IConfiguration config,
          ILogger<RabbitMQMessageBus> logger)
        {
            _logger = logger;

            // Read connection settings from appsettings.json
            //ConnectionFactory is a RabbitMQ class — it's the address book that tells your app where RabbitMQ is running.
            //config["RabbitMQ:Host"] reads from your appsettings.json:
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
                UserName = config["RabbitMQ:Username"] ?? "guest", //setting the rabbit mq username, you can change it accordingly in future
                Password = config["RabbitMQ:Password"] ?? "guest" ,//setting password
                  ClientProvidedName = "OrderAPI_Phase2"   // ← shows in RabbitMQ dashboard
            };

            //The `??` is the null-coalescing operator — it means:
            //"If the config value is missing/null, use this default instead"
            ///"If appsettings has no Host value, default to localhost"
            ///This is a safety net — your app won't crash if someone forgets to add the config.



           //EXPLANATION 2 - REFER WORD

            // Create connection synchronously in constructor
            // This runs ONCE at app startup — acceptable here
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();



            // explanation 3 - Declare exchange
            // durable: true = survives RabbitMQ restart
            // type Direct = routes by exact routing key match
            _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true
            ).GetAwaiter().GetResult();


            //SECTION - 4 - DECLARING THE QUEUE (REF NOTES FOR EXPLANATION )

            // Declare queue
            // durable: true = queue survives RabbitMQ restart
            // exclusive: false = other connections can use this queue
            // autoDelete: false = queue stays when no consumers connected
            _channel.QueueDeclareAsync(
                queue: QueueName, //// "order.created.queue"
                durable: true,
                exclusive: false,
                autoDelete: false
            ).GetAwaiter().GetResult();


            //SECTION 5 - BINDING QUEUE TO EXCHANGE - important step

            // Bind queue to exchange with routing key
            // Messages sent to "order.exchange" with key "order.created"
            // will be delivered to "order.created.queue"
            _channel.QueueBindAsync(
                queue: QueueName, //// "order.created.queue"
                exchange: ExchangeName, //// "order.exchange"
                routingKey: RoutingKey   // "order.created"
            ).GetAwaiter().GetResult();

            _logger.LogInformation(
                "✅ RabbitMQ connected. Exchange: {Exchange}, Queue: {Queue}",
                ExchangeName, QueueName);
        }


        public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent)
        {
            // Step 1: Serialize C# object → JSON string → byte array
            // RabbitMQ sends raw bytes — we convert our object to bytes
            var message = JsonSerializer.Serialize(orderCreatedEvent);
            var body = Encoding.UTF8.GetBytes(message);

            // Step 2: Set message properties
            var properties = new BasicProperties
            {
                // Persistent: true = message survives RabbitMQ restart
                // If false and RabbitMQ restarts, message is lost forever
                Persistent = true,
                ContentType = "application/json"
            };

            // Step 3: Publish to exchange with routing key
            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation(
                "📤 Published OrderCreatedEvent for Order {OrderId} to RabbitMQ",
                orderCreatedEvent.OrderId);
        }

        // Called automatically when app shuts down
        // Singleton is disposed on app shutdown
        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            _logger.LogInformation("🔌 RabbitMQ connection closed");
        }


    }
    }
