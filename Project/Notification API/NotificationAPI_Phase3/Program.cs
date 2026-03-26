using NotificationAPI_Phase3.Consumers;
using NotificationAPI_Phase3.Services;

var builder = WebApplication.CreateBuilder(args);

// NotificationService — Scoped
// New instance per scope (created manually inside each consumer)
builder.Services.AddScoped<NotificationService>();

// RabbitMQ Consumer — BackgroundService
// Listens to RabbitMQ queue continuously
builder.Services.AddHostedService<OrderCreatedConsumer>();

// Kafka Consumer — BackgroundService
// Listens to Kafka topic continuously
// Runs alongside RabbitMQ consumer simultaneously
builder.Services.AddHostedService<OrderKafkaConsumer>();

var app = builder.Build();

app.Run();