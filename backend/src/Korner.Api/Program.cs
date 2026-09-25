using Korner.Api.Extensions;
using Korner.Api.Logging;
using Sentry;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
}

// No-ops until Sentry:Dsn is configured (docs/SECRETS.md: not provided yet, due T7.7-T7.10).
SentrySdk.Init(options =>
{
    // An empty string (not null) is what tells the SDK to disable itself.
    options.Dsn = builder.Configuration["Sentry:Dsn"] ?? string.Empty;
    options.Environment = builder.Environment.EnvironmentName;
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.With<RedactSensitivePropertiesEnricher>()
        .WriteTo.Console(new JsonFormatter());
});

builder.Services.AddKorner(builder.Configuration);

var app = builder.Build();

app.UseKorner();

app.Run();

// Exposed so integration tests can bootstrap the app via WebApplicationFactory<Program>.
public partial class Program;
