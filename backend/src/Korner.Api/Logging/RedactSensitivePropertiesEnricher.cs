using Serilog.Core;
using Serilog.Events;

namespace Korner.Api.Logging;

/// <summary>
/// Redacts structured log properties whose name suggests PII or a secret (phone, email, token, ...)
/// before they reach any sink. Only catches values logged as named properties
/// (e.g. <c>Log.Information("Order for {Phone}", phone)</c>) — CLAUDE.md rule 11: backend logs
/// never contain tokens, phone numbers, emails or full addresses, so call sites must log these as
/// named properties rather than folding them into the free-text message.
/// </summary>
public sealed class RedactSensitivePropertiesEnricher : ILogEventEnricher
{
    private const string RedactedValue = "***REDACTED***";

    private static readonly string[] SensitiveNameFragments =
    [
        "phone", "email", "token", "password", "secret", "apikey", "hmac", "authorization",
    ];

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        foreach (var name in logEvent.Properties.Keys.ToList())
        {
            if (!IsSensitive(name))
            {
                continue;
            }

            logEvent.RemovePropertyIfPresent(name);
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(name, RedactedValue));
        }
    }

    private static bool IsSensitive(string propertyName) =>
        SensitiveNameFragments.Any(fragment => propertyName.Contains(fragment, StringComparison.OrdinalIgnoreCase));
}
