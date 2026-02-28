using NotificationAPI_Phase3.Consumers;
using NotificationAPI_Phase3.Services;

var builder = WebApplication.CreateBuilder(args);

// Notification.API is a background listener only
// No Swagger, no OpenAPI, no HTTP endpoints needed
builder.Services.AddScoped<NotificationService>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

app.Run();