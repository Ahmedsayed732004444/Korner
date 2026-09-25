using System.ComponentModel.DataAnnotations;

namespace Korner.Api.Configurations;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    [Required, Url]
    public string PublicBaseUrl { get; init; } = string.Empty;
}
