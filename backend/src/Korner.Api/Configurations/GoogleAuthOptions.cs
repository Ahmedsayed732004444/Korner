using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class GoogleAuthOptions
{
    public const string SectionName = "Authentication:Google";

    [Required]
    public string ClientId { get; init; } = string.Empty;

    [Required]
    public string ClientSecret { get; init; } = string.Empty;

    [Required]
    public string RedirectUri { get; init; } = string.Empty;

    [MinLength(1)]
    public string[] Scopes { get; init; } = [];
}
