namespace Korner.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseKorner(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Minimal placeholder for T0.4 (compose healthcheck + frontend probe). T1.4 replaces this
        // with the real /health that also checks DB connectivity and outbox lag.
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        app.MapControllers();

        return app;
    }
}
