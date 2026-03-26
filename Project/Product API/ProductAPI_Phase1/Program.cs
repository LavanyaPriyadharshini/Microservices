using Microsoft.EntityFrameworkCore;
using ProductAPI_Phase1.Data;
using ProductAPI_Phase1.Repositories;
using ProductAPI_Phase1.Repositories_Implementation;
using ProductAPI_Phase1.Services.Interfaces;
using ProductAPI_Phase1.Services.ServiceImplementation;

var builder = WebApplication.CreateBuilder(args);

// Load config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ── Controllers + Swagger ─────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Product API",
        Version = "v1",
        Description = "Product microservice for E-Commerce application"
    });
});

// ── EF Core DbContext ─────────────────────────────────────────
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProductDB"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    )
);


//this inmemory repo is used for the 
////builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();


// ── Repository ────────────────────────────────────────────────
builder.Services.AddScoped<IProductRepository, EFProductRepository>();

// ── Service ───────────────────────────────────────────────────
builder.Services.AddScoped<IProductService, ProductService>();

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Auto migrate on startup ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();
}

// ── Swagger only in Development ───────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();