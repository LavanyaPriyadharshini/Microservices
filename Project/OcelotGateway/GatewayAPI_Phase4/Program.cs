using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// ── Load ocelot.json configuration ────────────────────────────
// This tells .NET to read ocelot.json in addition to appsettings.json
// All routing rules are defined in ocelot.json
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);



// ── Register Ocelot ───────────────────────────────────────────
// AddOcelot reads all routes from ocelot.json
// and sets up the internal routing pipeline
builder.Services.AddOcelot(builder.Configuration);



var app = builder.Build();


// ── Use Ocelot Middleware ─────────────────────────────────────
// This is the core line — Ocelot intercepts ALL incoming requests
// matches them against routes in ocelot.json
// forwards them to the correct downstream service
// returns the response back to the client
await app.UseOcelot();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
