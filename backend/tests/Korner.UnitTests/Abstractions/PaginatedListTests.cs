using FluentAssertions;
using Korner.Api.Abstractions;

namespace Korner.UnitTests.Abstractions;

public class PaginatedListTests
{
    [Theory]
    [InlineData(1, 20, 45, 3, false, true)]
    [InlineData(3, 20, 45, 3, true, false)]
    [InlineData(2, 20, 45, 3, true, true)]
    [InlineData(1, 20, 0, 0, false, false)]
    public void Computed_properties_reflect_the_page_and_total_count(
        int page,
        int pageSize,
        int totalCount,
        int expectedTotalPages,
        bool expectedHasPrevious,
        bool expectedHasNext)
    {
        var list = new PaginatedList<int>([], page, pageSize, totalCount);

        list.TotalPages.Should().Be(expectedTotalPages);
        list.HasPreviousPage.Should().Be(expectedHasPrevious);
        list.HasNextPage.Should().Be(expectedHasNext);
    }
}
