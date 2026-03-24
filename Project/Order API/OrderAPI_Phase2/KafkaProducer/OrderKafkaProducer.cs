
using Confluent.Kafka;
using System.Text.Json;
using OrderAPI_Phase2.Events;
using System.Collections.Concurrent;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace OrderAPI_Phase2.KafkaProducer
{
    /// <summary>
    /// Kafka implementation of IOrderKafkaProducer.
    /// Registered as SINGLETON — Kafka producer is thread-safe
    /// and expensive to create, so one instance for app lifetime.
    /// </summary>
    public class OrderKafkaProducer : IOrderKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<OrderKafkaProducer> _logger;

        // Topic name — consumers must use this exact name(which means any services which needs to use this event must use this topic name)
        private const string TopicName = "order-created-topic";

        public OrderKafkaProducer(
          IConfiguration config,
          ILogger<OrderKafkaProducer> logger)
        {
            _logger = logger;

            // ProducerConfig — tells Kafka where the broker(server) is
            // and how to behave when publishing
            var producerConfig = new ProducerConfig
            {
                // Address of Kafka broker(server) running in this 9092 port in the docker container
                BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",

                // Acks.All = wait for ALL replicas(all servers, all clusters) to confirm message
                // Acks.Leader = wait only for leader broker(server) to confirm
                // Acks.None = don't wait for any confirmation (fastest but risky) from any server or broker
                // For learning: Leader is sufficient

                Acks = Acks.Leader,

                // Retry sending if broker is temporarily unavailable
                MessageSendMaxRetries = 3,

                // Wait 1 second between retries
                RetryBackoffMs = 1000
            };

            // Build the producer
            // string, string = Key type, Value type
            // Key   = used to determine which partition message goes to
            // Value = actual message content (JSON string)


            //kafka configuration stored in the variable _producer, this will be called later in the below code
            _producer = new ProducerBuilder<string, string>(producerConfig).Build();

            _logger.LogInformation(
               "✅ Kafka producer initialized. BootstrapServers: {Servers}",
               producerConfig.BootstrapServers);
        }
        

        //when this method is called from the order service or any other services, then the publishing takes place 
        //when this method is called , the kafka producer is called (refer orderservice.cs)

        public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent)
        {

            // Serialize event to JSON string
            var messageValue = JsonSerializer.Serialize(orderCreatedEvent);

            // Create Kafka message
            // Key   = OrderId.ToString() → ensures all events for same order
            //         go to same partition → maintains order of events
            // Value = JSON payload

            //(refer word notes)

            //here the message is created
            var message = new Message<string, string>
            {
                Key = orderCreatedEvent.OrderId.ToString(), // ← This determines PARTITION
                                                             //// ← Partition = hash(OrderId) % num_partitions
                Value = messageValue   // ← This is the actual MESSAGE
            };


            //how partition is created
// How it works behind the scenes:
//Kafka takes the Key = "123"(OrderId)
//Runs a hashing algorithm: hash("123") = 4582
//Does modulo: 4582 % 3 = 1(if topic has 3 partitions)
//Message goes to Partition 1


            // Publish to Kafka topic - this is publiching a message
            // waits for the broker to confirm receipt, it returns the delivery details
            //here _producer contains the kafka configuration
            //the above order message or any service message is published to a topic
            var deliveryResult = await _producer.ProduceAsync(TopicName, message);

            //kafka is an open source pplatform tool used in place of rabbit mq

        //What Kafka Does:
//        Topic: "order-created-topic"
//├── Partition 0: [Empty]
//├── Partition 1: [Key= 123, Offset = 156, Value ={
//                John's order}]  ← GOES HERE
//└── Partition 2: [Empty]

//                Why Partition 1 ? hash("123") = some number → maps to Partition 1 , using key partitions will be made


            //Logs exactly where the message landed:
            _logger.LogInformation(
              "📤 Kafka: Published OrderCreatedEvent. " +
              "OrderId: {OrderId}, Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
              orderCreatedEvent.OrderId,
              deliveryResult.Topic,
              deliveryResult.Partition.Value,
              deliveryResult.Offset.Value);

        }


        public void Dispose()
        {
            // Flush ensures all pending messages are sent before shutdown
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
            _logger.LogInformation("🔌 Kafka producer disposed");
        }


    }
    }
