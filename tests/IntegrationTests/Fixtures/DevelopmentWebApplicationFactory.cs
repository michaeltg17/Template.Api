using Microsoft.Extensions.Hosting;
using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Xunit.v3;

namespace IntegrationTests.Fixtures
{
    internal class DevelopmentWebApplicationFactory(ITestSettings testSettings, DatabaseFactory databaseFactory)
        : WebApplicationFactoryFixture(testSettings, databaseFactory, Environments.Development)
    {
    }
}