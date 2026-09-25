using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Korner.IntegrationTests;

public class ConfigPublicTests(KornerWebApplicationFactory factory) : IClassFixture<KornerWebApplicationFactory>
{
    [Fact]
    public async Task Reports_card_enabled_and_wallet_disabled_from_the_test_config()
    {
        // TestConfigurationDefaults: CardIntegrationId = 1, MobileIntegrationId = 0.
        var client = factory.CreateClient();

        var response = await client.GetAsync("/config/public");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ConfigPublicResponse>();
        body.Should().NotBeNull();
        body!.CardEnabled.Should().BeTrue();
        body.WalletEnabled.Should().BeFalse();
    }

    private sealed record ConfigPublicResponse(bool WalletEnabled, bool CardEnabled);
}
