using System.Text;
using NotificationAPI_Phase3.Models;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationAPI_Phase3.Services;

namespace NotificationAPI_Phase3.Consumers
{

    /// <summary>
    /// BackgroundService that continuously listens for messages from RabbitMQ.
    ///
    /// BackgroundService is a built-in .NET class that runs a long-running task
    /// when your app starts, in the background, separate from HTTP request handling.
    ///
    /// Lifecycle:
    /// App starts → StartAsync() → ExecuteAsync() listens forever → App stops → StopAsync()
    /// </summary>
    /// 
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IChannel? _channel;

        // Must match EXACTLY what Order.API declared
        // If names don't match → messages will never be received
        private const string ExchangeName = "order.exchange";
        private const string QueueName = "order.created.queue";
        private const string RoutingKey = "order.created";

        /// <summary>
        /// Why IServiceScopeFactory instead of injecting NotificationService directly?
        ///
        /// BackgroundService = Singleton (lives entire app lifetime)
        /// NotificationService = Scoped (lives per request/scope)
        ///
        /// You CANNOT inject Scoped into Singleton directly.
        /// It causes "Captive Dependency" — scoped service gets trapped
        /// inside singleton and never released = memory leak.
        ///
        /// Solution: inject IServiceScopeFactory → create scope manually
        /// each time a message arrives → resolve NotificationService inside scope
        /// → scope is disposed after processing → no memory leak
        /// </summary>
        /// 


        public OrderCreatedConsumer(
         IConfiguration config,
         ILogger<OrderCreatedConsumer> logger,
         IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }


        /// <summary>
        /// Called ONCE when app starts.
        /// Sets up RabbitMQ connection, exchange, queue, binding.
        /// </summary>
        /// 

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
                UserName = _config["RabbitMQ:Username"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest",
                  ClientProvidedName = "NotificationAPI_Phase3" //this shows in the rabbit mq
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declare same exchange and queue as Order.API
            // RabbitMQ is idempotent — safe to declare already existing ones
            // This ensures queue exists even if Notification.API starts before Order.API

            await _channel.ExchangeDeclareAsync(
              exchange: ExchangeName,
              type: ExchangeType.Direct,
              durable: true,
              cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
               queue: QueueName,
               durable: true,
               exclusive: false,
               autoDelete: false,
               cancellationToken: cancellationToken);


            await _channel.QueueBindAsync(
             queue: QueueName,
             exchange: ExchangeName,
             routingKey: RoutingKey,
             cancellationToken: cancellationToken);

            // prefetchCount: 1 → process ONE message at a time
            // Prevents this instance from grabbing all messages at once
            // Fair dispatch — other consumers get messages too

            await _channel.BasicQosAsync(
               prefetchSize: 0,
               prefetchCount: 1,
               global: false,
               cancellationToken: cancellationToken);

            _logger.LogInformation(
               "✅ Notification.API connected to RabbitMQ. Listening on queue: {Queue}",
               QueueName);

            await base.StartAsync(cancellationToken);

        }

        /// <summary>
        /// The main listening loop.
        /// Registers a callback that fires every time a message arrives.
        /// </summary>
        /// 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // This fires every time a message arrives in the queue
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("📥 Message received from queue: {Queue}", QueueName);

                try
                {
                    // Deserialize JSON bytes → C# object
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true  // handles camelCase/PascalCase
                        });

                    if (orderEvent is null)
                    {
                        _logger.LogWarning("Received null message, skipping");
                        await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                        return;
                    }

                    // Create scope → resolve NotificationService → process → dispose scope
                    using var scope = _scopeFactory.CreateScope();
                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<NotificationService>();

                    await notificationService.SendOrderConfirmationAsync(orderEvent);

                    // ACK = "I processed this successfully, remove from queue"
                    // Without ACK → RabbitMQ keeps message and redelivers it

                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

                    _logger.LogInformation(
                      "✅ Message acknowledged for Order {OrderId}",
                      orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error processing message: {Message}", message);

                    // NACK = "I failed to process this"
                    // requeue: false → don't retry infinitely on persistent errors
                    //                  goes to Dead Letter Queue if configured
                    await _channel!.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false);
                }
            };

            // Start consuming messages
            // autoAck: false → we manually send ACK after successful processing
            // autoAck: true  → message deleted immediately on delivery
            //                   even if our code crashes = data loss ❌

            await _channel!.BasicConsumeAsync(
             queue: QueueName,
             autoAck: false,
             consumer: consumer,
             cancellationToken: stoppingToken);

            // Keep running until app stops
            await Task.Delay(Timeout.Infinite, stoppingToken);
                    }


        /// <summary>
        /// Called when app is shutting down. Clean up connections.
        /// </summary>
        /// 

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
                await _channel.CloseAsync(cancellationToken);

            if (_connection is not null)
                await _connection.CloseAsync(cancellationToken);

            _logger.LogInformation("🔌 Notification.API disconnected from RabbitMQ");

            await base.StopAsync(cancellationToken);
        }
    }
}
