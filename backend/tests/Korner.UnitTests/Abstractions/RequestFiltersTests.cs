using FluentAssertions;
using Korner.Api.Abstractions;

namespace Korner.UnitTests.Abstractions;

public class RequestFiltersTests
{
    [Fact]
    public void Defaults_to_page_1_and_page_size_20()
    {
        var filters = new RequestFilters();

        filters.Page.Should().Be(1);
        filters.PageSize.Should().Be(20);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Page_ignores_non_positive_values(int invalidPage)
    {
        var filters = new RequestFilters { Page = invalidPage };

        filters.Page.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(RequestFilters.MaxPageSize + 1)]
    public void PageSize_ignores_values_outside_the_allowed_range(int invalidPageSize)
    {
        var filters = new RequestFilters { PageSize = invalidPageSize };

        filters.PageSize.Should().Be(20);
    }

    [Fact]
    public void PageSize_accepts_the_maximum_allowed_value()
    {
        var filters = new RequestFilters { PageSize = RequestFilters.MaxPageSize };

        filters.PageSize.Should().Be(RequestFilters.MaxPageSize);
    }
}
