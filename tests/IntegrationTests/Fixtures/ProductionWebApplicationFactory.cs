using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;

namespace IntegrationTests.Fixtures
{
    internal class ProductionWebApplicationFactory(
        ITestSettings testSettings,
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock,
        Database database)
        : WebApplicationFactory(testSettings, inMemorySink, injectableTestOutputSink, imageApiMock, database)
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(Environments.Production);

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false);
            });

            return base.CreateHost(builder);
        }
    }
}