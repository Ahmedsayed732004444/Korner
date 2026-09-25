using Korner.Api.Configurations;
using Microsoft.Extensions.Options;

namespace Korner.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseKorner(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseOutputCache();

        WarnAboutDisabledIntegrations(app);

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Minimal placeholder for T0.4 (compose healthcheck + frontend probe). T1.4 replaces this
        // with the real /health that also checks DB connectivity and outbox lag.
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        // maintenanceMode / freeShippingThresholdPiasters / whatsAppNumber (technical design §6)
        // land once the Setting entity exists (T1.3) — only the Paymob-derived flags are real today.
        app.MapGet("/config/public", (IOptions<PaymobSettingsOptions> paymobOptions) =>
            {
                var paymob = paymobOptions.Value;
                return Results.Ok(new
                {
                    walletEnabled = paymob.MobileIntegrationId != 0,
                    cardEnabled = paymob.CardIntegrationId != 0,
                });
            })
            .CacheOutput(policy => policy.Expire(TimeSpan.FromSeconds(60)));

        app.MapControllers();

        return app;
    }

    private static void WarnAboutDisabledIntegrations(WebApplication app)
    {
        var paymob = app.Services.GetRequiredService<IOptions<PaymobSettingsOptions>>().Value;

        if (paymob.MobileIntegrationId == 0)
        {
            app.Logger.LogWarning(
                "Wallet payments are disabled: PaymobSettings:MobileIntegrationId is 0. " +
                "Create a Mobile Wallet integration in the Paymob dashboard and set its ID to enable wallets.");
        }
    }
}
