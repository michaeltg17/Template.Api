using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;

namespace IntegrationTests.Fixtures
{
    internal class ProductionWebApplicationFactory(ITestSettings testSettings, DatabaseFactory databaseFactory)
        : WebApplicationFactoryFixture(testSettings, databaseFactory, Environments.Production)
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false);
            });

            return base.CreateHost(builder);
        }
    }
}