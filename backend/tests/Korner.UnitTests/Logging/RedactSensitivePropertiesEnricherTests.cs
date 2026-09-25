using FluentAssertions;
using Korner.Api.Logging;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Korner.UnitTests.Logging;

public class RedactSensitivePropertiesEnricherTests
{
    private sealed class PassthroughPropertyFactory : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false) =>
            new(name, new ScalarValue(value));
    }

    private static readonly ILogEventPropertyFactory PropertyFactory = new PassthroughPropertyFactory();

    [Theory]
    [InlineData("Phone")]
    [InlineData("Email")]
    [InlineData("Token")]
    [InlineData("customerPassword")]
    [InlineData("Authorization")]
    public void Redacts_properties_whose_name_looks_sensitive(string propertyName)
    {
        var logEvent = CreateLogEvent(propertyName, "01012345678");
        var enricher = new RedactSensitivePropertiesEnricher();

        enricher.Enrich(logEvent, PropertyFactory);

        var value = logEvent.Properties[propertyName].Should().BeOfType<ScalarValue>().Subject;
        value.Value.Should().Be("***REDACTED***");
    }

    [Fact]
    public void Leaves_non_sensitive_properties_untouched()
    {
        var logEvent = CreateLogEvent("OrderNumber", "K-260101-0001");
        var enricher = new RedactSensitivePropertiesEnricher();

        enricher.Enrich(logEvent, PropertyFactory);

        var value = logEvent.Properties["OrderNumber"].Should().BeOfType<ScalarValue>().Subject;
        value.Value.Should().Be("K-260101-0001");
    }

    private static LogEvent CreateLogEvent(string propertyName, string value)
    {
        var template = new MessageTemplateParser().Parse($"Test {{{propertyName}}}");
        var property = new LogEventProperty(propertyName, new ScalarValue(value));

        return new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, template, [property]);
    }
}
