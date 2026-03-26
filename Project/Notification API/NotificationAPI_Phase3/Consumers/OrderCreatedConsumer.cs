using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;
using NotificationAPI_Phase3.Models;
using NotificationAPI_Phase3.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;



namespace NotificationAPI_Phase3.Consumers
{


    /// <summary>
    /// BackgroundService that continuously listens to RabbitMQ queue.
    /// Runs automatically when app starts.
    /// </summary>
    /// 
    public class OrderCreatedConsumer : BackgroundService
    {

        private readonly IConfiguration _config;
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;


        private IConnection? _connection;
        private IChannel? _channel;

        private const string ExchangeName = "order.exchange";
        private const string QueueName = "order.created.queue";
        private const string RoutingKey = "order.created";


        public OrderCreatedConsumer(
         IConfiguration config,
         ILogger<OrderCreatedConsumer> logger,
         IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }


        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
                UserName = _config["RabbitMQ:Username"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest",
                ClientProvidedName = "NotificationAPI_Phase3"
            };

            // Retry logic — RabbitMQ takes time to initialize in Docker (refer polly and circuit breaker concept for this )
            var retryCount = 10;
            var retryDelay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    _logger.LogInformation(
                        "Attempting RabbitMQ connection... (Attempt {Attempt}/{Total})",
                        i + 1, retryCount);

                    _connection = await factory.CreateConnectionAsync(cancellationToken);
                    _channel = await _connection.CreateChannelAsync(
                        cancellationToken: cancellationToken);
                    break;
                }
                catch (Exception ex)
                {
                    if (i == retryCount - 1) throw;
                    _logger.LogWarning(
                        "⏳ RabbitMQ not ready. Retrying in {Delay}s... (Attempt {Attempt}/{Total})",
                        retryDelay.TotalSeconds, i + 1, retryCount);
                    await Task.Delay(retryDelay, cancellationToken);
                }
            }

            await _channel!.ExchangeDeclareAsync(
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


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation(
                    "📥 RabbitMQ: Message received from queue: {Queue}", QueueName);

                try
                {
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                        message,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (orderEvent is null)
                    {
                        _logger.LogWarning("Received null message, skipping");
                        await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<NotificationService>();

                    await notificationService.SendOrderConfirmationAsync(orderEvent);

                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

                    _logger.LogInformation(
                        "✅ RabbitMQ: Message acknowledged for Order {OrderId}",
                        orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ RabbitMQ: Error processing message");
                    await _channel!.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false);
                }
            };

            await _channel!.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

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
