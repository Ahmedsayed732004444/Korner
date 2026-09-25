using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Korner.IntegrationTests;

/// <summary>
/// Boots the real app with <see cref="TestConfigurationDefaults"/> layered on top of (and
/// overriding) whatever appsettings.Local.json/environment already provides, so tests never
/// depend on — or accidentally use — a developer's real local secrets.
/// </summary>
public class KornerWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(TestConfigurationDefaults.Values));
    }
}
