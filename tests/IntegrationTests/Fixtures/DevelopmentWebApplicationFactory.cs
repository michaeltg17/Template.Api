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
        : WebApplicationFactory(testSettings, databaseFactory, Environments.Development)
    {
    }
}