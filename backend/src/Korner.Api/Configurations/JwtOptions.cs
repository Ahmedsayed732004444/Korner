using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, MinLength(32)]
    public string Key { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int AccessTokenMinutes { get; init; }

    [Range(1, int.MaxValue)]
    public int RefreshTokenDays { get; init; }
}
