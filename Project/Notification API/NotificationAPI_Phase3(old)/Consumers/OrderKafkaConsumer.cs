using System.Text.Json;
using Confluent.Kafka;
using NotificationAPI_Phase3.Models;
using NotificationAPI_Phase3.Services;
using static Confluent.Kafka.ConfigPropertyNames;

namespace NotificationAPI_Phase3.Consumers
{
    /// <summary>
    /// BackgroundService that continuously listens to Kafka topic.
    /// 
    /// Kafka consumer differences from RabbitMQ consumer:
    /// - Kafka tracks offset (position) not acknowledgements
    /// - Messages stay in Kafka after consuming (not deleted)
    /// - Consumer pulls messages (RabbitMQ pushes messages)
    /// - Consumer group enables parallel processing
    /// </summary>
    /// 


    public class OrderKafkaConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<OrderKafkaConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        // Must match exactly what Order.API publishes to Kafka , refer orderservice.cs kafka producer class
        private const string TopicName = "order-created-topic";

        // Consumer Group ID
        // All instances of Notification.API share this group ID
        // Kafka distributes partitions among group members
        // Each message processed by only ONE instance in the group

        private const string GroupId = "notification-consumer-group";


        public OrderKafkaConsumer(
           IConfiguration config,
           ILogger<OrderKafkaConsumer> logger,
           IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ConsumerConfig — tells Kafka how to behave as a consumer
            var consumerConfig = new ConsumerConfig
            {
                // Address of Kafka broker
                BootstrapServers = _config["Kafka:BootstrapServers"] ?? "localhost:9092",


                // Group ID — identifies this consumer as part of a group
                GroupId = GroupId,

                // AutoOffsetReset: how the messages will be read
                // Earliest = start from beginning of topic if no offset saved
                // Latest   = start from newest messages only ( previous old messages will not be considered)
                // Use Earliest for learning — you won't miss any messages

                AutoOffsetReset = AutoOffsetReset.Earliest,


                // EnableAutoCommit: false = we manually commit offset
                // after successful processing (same as autoAck:false in RabbitMQ)
                // If true = offset committed immediately on receive
                //           even if processing crashes = message lost ❌

                EnableAutoCommit = false

            };


            // Build the consumer
            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();


            // Subscribe to topic
            // Kafka automatically assigns partitions to this consumer
            //tells kafka "i want messages from this topic"
            //kafka automatically assigns partition to this consumer.
            consumer.Subscribe(TopicName);


            _logger.LogInformation(
                "✅ Kafka consumer started. Topic: {Topic}, Group: {Group}",
                TopicName, GroupId);

            // Keep consuming until app stops
            //the listening heart
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Consume() PULLS next message from Kafka
                    // TimeSpan.FromSeconds(1) = wait up to 1 second for a message
                    // If no message arrives in 1 second → returns null → loop continues
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

                    // No message arrived in this iteration → try again
                    if (consumeResult is null) continue;

                    _logger.LogInformation(
                       "📥 Kafka: Message received. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                       consumeResult.Topic,
                       consumeResult.Partition.Value,
                       consumeResult.Offset.Value);

                    //processing each message received from kafka, kafak usually have json texts, that we want to convert to the csharp objects
                    // Deserialize JSON string → C# object
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                        consumeResult.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (orderEvent is null)
                    {
                        _logger.LogWarning("Received null Kafka message, skipping");

                        // Commit offset anyway to move past bad message
                        //tells kafka "processed successfully move forward"
                        //if app crashes before commit -> same message reprocessed on restart
                        //if exception -> dont commit -> retry after 1 second
                        consumer.Commit(consumeResult);
                        continue;
                    }

                    // Create scope → resolve NotificationService → process → dispose
                    using var scope = _scopeFactory.CreateScope();
                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<NotificationService>();


                    await notificationService.SendOrderConfirmationAsync(orderEvent);

                    // Commit offset = "I have successfully processed up to this point"
                    // Kafka stores this offset for this consumer group
                    // If app restarts → resumes from this offset

                    consumer.Commit(consumeResult);


                    _logger.LogInformation(
                       "✅ Kafka: Offset committed. Partition: {Partition}, Offset: {Offset}",
                       consumeResult.Partition.Value,
                       consumeResult.Offset.Value);

                }
                catch (OperationCanceledException)
                {
                    // App is shutting down → exit loop cleanly
                    break;
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Kafka: Error processing message");
                    // Don't commit offset → message will be reprocessed on restart
                    await Task.Delay(1000, stoppingToken); // wait before retrying
                }
            }

            // Clean up — unsubscribe and close consumer
            //clean disconnection
            consumer.Unsubscribe();
            consumer.Close();
            _logger.LogInformation("🔌 Kafka consumer stopped");
        }
    }

    //RabbitMQ → broker PUSHES messages to consumer
    //Kafka    → consumer PULLS messages from broker
}