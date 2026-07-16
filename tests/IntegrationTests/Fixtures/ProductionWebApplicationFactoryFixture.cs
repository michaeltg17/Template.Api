using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Xunit.v3;

namespace IntegrationTests.Fixtures
{
    internal class ProductionWebApplicationFactoryFixture(ITestSettings testSettings, DatabaseFactory databaseFactory)
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