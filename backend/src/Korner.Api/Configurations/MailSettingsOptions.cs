using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class MailSettingsOptions
{
    public const string SectionName = "MailSettings";

    [Required, EmailAddress]
    public string Mail { get; init; } = string.Empty;

    [Required]
    public string DisplayName { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; }
}
