using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load correct ocelot config based on environment
// Development → ocelot.json        (localhost URLs)
// Production  → ocelot.Docker.json (service names)
var ocelotFile = builder.Environment.IsDevelopment()
    ? "ocelot.json"
    : "ocelot.Docker.json";

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(ocelotFile, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

await app.UseOcelot();

app.Run();