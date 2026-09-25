using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class HangfireSettingsOptions
{
    public const string SectionName = "HangfireSettings";

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
