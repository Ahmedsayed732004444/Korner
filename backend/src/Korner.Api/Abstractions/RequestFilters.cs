namespace Korner.Api.Abstractions;

/// <summary>Base query parameters for paginated list endpoints. Feature-specific filters extend this.</summary>
public record RequestFilters
{
    public const int MaxPageSize = 100;

    private readonly int _page = 1;
    private readonly int _pageSize = 20;

    public int Page
    {
        get => _page;
        init => _page = value > 0 ? value : _page;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value is > 0 and <= MaxPageSize ? value : _pageSize;
    }
}
