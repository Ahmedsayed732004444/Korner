namespace Korner.IntegrationTests;

/// <summary>
/// Valid-but-fake values for every option T1.2 requires at startup. CI has no real secrets
/// (docs/SECRETS.md rule 4) and appsettings.Local.json isn't part of the repo, so tests supply
/// their own baseline via <see cref="KornerWebApplicationFactory"/> instead.
/// </summary>
public static class TestConfigurationDefaults
{
    public static Dictionary<string, string?> Values => new()
    {
        ["Jwt:Key"] = "test-signing-key-at-least-32-bytes-long!!",
        ["Jwt:Issuer"] = "Korner.Tests",
        ["Jwt:Audience"] = "Korner.Tests",
        ["Jwt:AccessTokenMinutes"] = "15",
        ["Jwt:RefreshTokenDays"] = "15",

        ["Auth:CookieDomain"] = "localhost",
        ["Auth:StorefrontOrigins:0"] = "http://localhost:5173",

        ["Authentication:Google:ClientId"] = "test-client-id",
        ["Authentication:Google:ClientSecret"] = "test-client-secret",
        ["Authentication:Google:RedirectUri"] = "/signin-google",
        ["Authentication:Google:Scopes:0"] = "openid",

        ["MailSettings:Mail"] = "test@example.com",
        ["MailSettings:DisplayName"] = "Korner Test",
        ["MailSettings:Password"] = "test-password",
        ["MailSettings:Host"] = "smtp.example.com",
        ["MailSettings:Port"] = "587",

        ["PaymobSettings:APIKey"] = "test-api-key",
        ["PaymobSettings:SecretKey"] = "test-secret-key",
        ["PaymobSettings:PublicKey"] = "test-public-key",
        ["PaymobSettings:HMAC"] = "test-hmac",
        ["PaymobSettings:CardIntegrationId"] = "1",
        ["PaymobSettings:MobileIntegrationId"] = "0",
        ["PaymobSettings:BaseUrl"] = "https://accept.paymob.com/api/",
        ["PaymobSettings:ApiPublicUrl"] = "https://test.example.com",
        ["PaymobSettings:StorefrontUrl"] = "https://test.example.com",
        ["PaymobSettings:InquiryPath"] = "api/ecommerce/orders/transaction_inquiry",
        ["PaymobSettings:ReservationMinutes"] = "15",
        ["PaymobSettings:MaxAttemptsPerOrder"] = "5",

        ["HangfireSettings:Username"] = "test-admin",
        ["HangfireSettings:Password"] = "test-password",

        ["Storage:PublicBaseUrl"] = "https://localhost:5081",
    };
}
