using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Korner.IntegrationTests;

public class OptionsValidationTests
{
    [Fact]
    public void App_refuses_to_start_when_a_required_option_is_missing()
    {
        // A blank value, not an omitted key: omitting a key only proves it wasn't set by this
        // particular source, and another source (e.g. a developer's local appsettings.Local.json)
        // could still supply it — this must fail regardless of what else is configured.
        var brokenValues = new Dictionary<string, string?>(TestConfigurationDefaults.Values)
        {
            ["Jwt:Key"] = string.Empty,
        };

        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(brokenValues)));

        var act = () => factory.CreateClient();

        act.Should()
            .Throw<OptionsValidationException>()
            .Where(exception => exception.Message.Contains("Jwt", StringComparison.Ordinal));
    }
}
