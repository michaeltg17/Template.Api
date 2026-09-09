using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;

namespace IntegrationTests.Fixtures
{
    public class UnhealthyTestFixture(
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock,
        ITestSettings testSettings,
        DatabaseFactory databaseFactory)
        : TestFixture(inMemorySink, injectableTestOutputSink, imageApiMock, testSettings, databaseFactory)
    {
        const string UnreachableConnectionString = "Host=localhost;Port=1;Database=template_db;Username=postgres;Password=postgres;";

        protected override Task<Database> CreateDatabase()
        {
            return Task.FromResult(new Database(null) { ConnectionString = UnreachableConnectionString });
        }
    }
}
