using Ocelot.DependencyInjection; // ← Import for Ocelot services
using Ocelot.Middleware;          // ← Import for Ocelot middleware

var builder = WebApplication.CreateBuilder(args);

// ════════════════════════════════════════════════════════════════════════════
// OCELOT CONFIGURATION FILE SETUP
// ════════════════════════════════════════════════════════════════════════════
// Tell ASP.NET Core to load Ocelot's routing configuration file
//
// Why separate config file?
// - Keeps routing rules separate from app settings
// - Easy to manage many routes without cluttering Program.cs
// - Can swap config files for different environments (dev/staging/prod)
//
// Configuration file loaded: ocelot.Docker.json
// This file contains:
// - All route mappings (upstream → downstream)
// - Service addresses (product-api:8080, order-api:8080)
// - HTTP methods allowed (GET, POST, PUT, DELETE)
//
// Alternative approach (not used here):
// You could load different configs based on environment:
// - ocelot.Development.json (for local development)
// - ocelot.Docker.json (for Docker environment)
// - ocelot.Production.json (for production deployment)
builder.Configuration.AddJsonFile(
    "ocelot.Docker.json",  // File name
    optional: false,        // File must exist (app won't start without it)
    reloadOnChange: true    // Hot reload: changes apply without restart
);


// ════════════════════════════════════════════════════════════════════════════
// OCELOT SERVICE REGISTRATION
// ════════════════════════════════════════════════════════════════════════════
// Register all Ocelot services into Dependency Injection container
// This includes:
// - HTTP client factory for calling downstream services
// - Route matching engine
// - Request transformation pipeline
// - Response aggregation (if configured)
// - Load balancer (if configured)
// - Service discovery integration (if configured)
builder.Services.AddOcelot();


// ════════════════════════════════════════════════════════════════════════════
// OPTIONAL: Add CORS if your gateway needs to handle browser requests
// ════════════════════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// ════════════════════════════════════════════════════════════════════════════
// BUILD THE APPLICATION
// ════════════════════════════════════════════════════════════════════════════
var app = builder.Build();


// ════════════════════════════════════════════════════════════════════════════
// MIDDLEWARE PIPELINE CONFIGURATION
// ════════════════════════════════════════════════════════════════════════════

// Enable CORS (if added above)
app.UseCors("AllowAll");


// ════════════════════════════════════════════════════════════════════════════
// OCELOT MIDDLEWARE - THIS IS THE MAGIC! ✨
// ════════════════════════════════════════════════════════════════════════════
// UseOcelot() activates the API Gateway functionality
//
// What happens when a request arrives:
// 1. Client sends: GET http://gateway:8080/api/products
// 2. Ocelot matches this to a route in ocelot.Docker.json
// 3. Finds matching route:
//    {
//      "UpstreamPathTemplate": "/api/products",
//      "UpstreamHttpMethod": ["GET"],
//      "DownstreamPathTemplate": "/api/product/getallproducts",
//      "DownstreamScheme": "http",
//      "DownstreamHostAndPorts": [{"Host": "product-api", "Port": 8080}]
//    }
// 4. Transforms request to: GET http://product-api:8080/api/product/getallproducts
// 5. Calls Product API
// 6. Receives response from Product API
// 7. Returns response to client
//
// All of this happens AUTOMATICALLY - you don't write routing code!
await app.UseOcelot();

// Start the web server and keep it running
app.Run();


// ════════════════════════════════════════════════════════════════════════════
// 📚 GATEWAY ARCHITECTURE EXPLAINED
// ════════════════════════════════════════════════════════════════════════════
//
// WITHOUT GATEWAY (Client talks to each service):
// ┌──────────┐       ┌──────────────┐
// │  Client  │──────▶│ Product API  │ http://product-api:8080
// │          │       └──────────────┘
// │          │       ┌──────────────┐
// │          │──────▶│  Order API   │ http://order-api:8080
// └──────────┘       └──────────────┘
//
// Problems:
// - Client needs to know ALL service URLs
// - Changes in service URLs require client updates
// - Difficult to add authentication, rate limiting per service
// - CORS configuration needed on each service
//
//
// WITH GATEWAY (Client talks only to gateway):
// ┌──────────┐       ┌─────────────┐       ┌──────────────┐
// │  Client  │──────▶│   GATEWAY   │──────▶│ Product API  │
// │          │       │  (Ocelot)   │       └──────────────┘
// │          │       │             │       ┌──────────────┐
// │          │       │             │──────▶│  Order API   │
// └──────────┘       └─────────────┘       └──────────────┘
//                    Single Entry Point
//                    http://gateway:8080
//
// Benefits:
// - Client only knows ONE URL: http://gateway:8080
// - Services can be added/removed/moved without client changes
// - Centralized authentication, rate limiting, caching
// - Single CORS configuration
// - Can aggregate responses from multiple services
// - Load balancing across service instances
//
//
// REQUEST FLOW EXAMPLE:
// 1. Mobile App → GET http://gateway:8080/api/products
// 2. Gateway → GET http://product-api:8080/api/product/getallproducts
// 3. Product API → Returns product list
// 4. Gateway → Returns product list to Mobile App
//
// 5. Mobile App → POST http://gateway:8080/api/orders
// 6. Gateway → POST http://order-api:8080/api/order
// 7. Order API → Creates order, publishes to Kafka
// 8. Gateway → Returns order confirmation to Mobile App
//
// Meanwhile (asynchronously):
// 9. Kafka → Notification API consumes order event
// 10. Notification API → Sends confirmation (email/SMS/console log)
//
// ════════════════════════════════════════════════════════════════════════════
// 🎯 KEY TAKEAWAYS
// ════════════════════════════════════════════════════════════════════════════
//
// 1. GATEWAY = Single entry point for ALL clients
// 2. ROUTING = Defined in ocelot.Docker.json (not in code)
// 3. TRANSPARENT = Clients don't know services exist behind gateway
// 4. SCALABLE = Easy to add new services without changing clients
// 5. SECURE = Can add authentication once at gateway level
//
// ════════════════════════════════════════════════════════════════════════════

// NOTE: In Ocelot 23.0+, UseOcelot() configures the middleware but doesn't block.
// We need app.Run() to actually start the web server and keep it running.
// The application will now listen on port 8080 and route requests through Ocelot.
