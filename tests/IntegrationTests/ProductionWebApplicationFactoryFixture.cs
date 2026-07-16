using Microsoft.Extensions.Hosting;
using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Xunit.v3;

namespace IntegrationTests
{
    internal class ProductionWebApplicationFactoryFixture(ITestSettings testSettings, DatabaseFactory databaseFactory)
        : WebApplicationFactoryFixture(testSettings, databaseFactory, Environments.Production)
    {
    }
}