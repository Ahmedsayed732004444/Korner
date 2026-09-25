using FluentAssertions;

namespace Korner.UnitTests;

public class SmokeTests
{
    [Fact]
    public void Test_project_runs()
    {
        var result = 1 + 1;

        result.Should().Be(2);
    }
}
