using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class PaymobSettingsOptions
{
    public const string SectionName = "PaymobSettings";

    [Required]
    public string APIKey { get; init; } = string.Empty;

    [Required]
    public string SecretKey { get; init; } = string.Empty;

    [Required]
    public string PublicKey { get; init; } = string.Empty;

    [Required]
    public string HMAC { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CardIntegrationId { get; init; }

    // 0 = wallets disabled (technical design §2.11 "Feature availability") — not a validation error.
    [Range(0, int.MaxValue)]
    public int MobileIntegrationId { get; init; }

    // Legacy iframe / auth-capture flow: keep the keys, never used (technical design §4.1).
    public int IframeId { get; init; }

    public int AuthCaptureIntegrationId { get; init; }

    [Required, Url]
    public string BaseUrl { get; init; } = string.Empty;

    [Required]
    public string ApiPublicUrl { get; init; } = string.Empty;

    [Required, Url]
    public string StorefrontUrl { get; init; } = string.Empty;

    [Required]
    public string InquiryPath { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ReservationMinutes { get; init; }

    [Range(1, int.MaxValue)]
    public int MaxAttemptsPerOrder { get; init; }
}
