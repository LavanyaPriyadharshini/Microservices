using OrderAPI_Phase2.HttpClients.Implementation;
using OrderAPI_Phase2.HttpClients;
using OrderAPI_Phase2.Repositories.InterfaceRepo;
using OrderAPI_Phase2.Repositories.ReposImplementation;
using OrderAPI_Phase2.Services.Interfaces;
using OrderAPI_Phase2.Services.ServiceImplementation;
using Polly.Extensions.Http;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Register repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register services
builder.Services.AddScoped<IOrderService, OrderService>();


// Configure HttpClient for Product.API with Polly resilience policies
builder.Services.AddHttpClient<IProductHttpClient, ProductHttpClient>(client =>
{
    // Base URL for Product.API (from appsettings.json)
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(GetRetryPolicy())      // Retry on transient failures
.AddPolicyHandler(GetCircuitBreakerPolicy()); // Circuit breaker


// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Order API",
        Version = "v1",
        Description = "Order microservice for E-Commerce application - Calls Product API"
    });
});



// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


// Polly Policies

/// <summary>
/// Retry policy: Retry 3 times with exponential backoff
/// </summary>
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 5xx and 408 errors
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds}s due to: {outcome.Result?.StatusCode}");
            });
}



/// <summary>
/// Circuit breaker: Stop calling if 5 consecutive failures, wait 30 seconds
/// </summary>
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, duration) =>
            {
                Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds}s");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit breaker reset");
            });
}


//### 🧠 Understanding Polly Policies:

//**Retry Policy: **
//```
//Request fails → Wait 2s → Retry
//Fails again → Wait 4s → Retry  
//Fails again → Wait 8s → Retry
//Still fails → Throw exception
//```

//**Circuit Breaker:**
//```
//5 failures in a row → Open circuit (stop calling Product.API)
//Wait 30 seconds → Try one request (half-open)
//Success? → Close circuit (resume normal)
//Failure? → Open circuit again