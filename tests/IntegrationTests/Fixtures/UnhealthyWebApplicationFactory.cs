using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;

namespace IntegrationTests.Fixtures
{
    internal class UnhealthyWebApplicationFactory(
        ITestSettings testSettings,
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock,
        Database database)
        : WebApplicationFactory(testSettings, inMemorySink, injectableTestOutputSink, imageApiMock, database)
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                services.AddHealthChecks().AddCheck("unhealthy-test", () => HealthCheckResult.Unhealthy()));

            return base.CreateHost(builder);
        }
    }
}
