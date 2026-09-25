using System.Net;
using FluentAssertions;

namespace Korner.IntegrationTests;

public class SmokeTests(KornerWebApplicationFactory factory) : IClassFixture<KornerWebApplicationFactory>
{
    [Fact]
    public async Task Host_boots_and_handles_a_request()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
