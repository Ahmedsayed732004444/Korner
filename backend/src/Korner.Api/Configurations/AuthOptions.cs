using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    [Required]
    public string CookieDomain { get; init; } = string.Empty;

    [MinLength(1)]
    public string[] StorefrontOrigins { get; init; } = [];
}
