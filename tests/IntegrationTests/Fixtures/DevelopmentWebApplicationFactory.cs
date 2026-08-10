using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;

namespace IntegrationTests.Fixtures
{
    internal class DevelopmentWebApplicationFactory(
        ITestSettings testSettings,
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock,
        Database database)
        : WebApplicationFactory(testSettings, inMemorySink, injectableTestOutputSink, imageApiMock, database)
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(Environments.Development);
            return base.CreateHost(builder);
        }
    }
}