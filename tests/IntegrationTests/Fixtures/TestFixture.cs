using IntegrationTests.Infrastructure;
using Serilog;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;
using Xunit;

namespace IntegrationTests.Fixtures
{
    internal class TestFixture(
        DatabaseFactory databaseFactory,
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock) : IAsyncLifetime
    {
        public WebApplicationFactory WebApplicationFactory { get; set; } = default!;
        public InMemorySink InMemorySink { get; } = inMemorySink;
        public InjectableTestOutputSink InjectableTestOutputSink { get; set; } = injectableTestOutputSink;
        internal ImageApiMock ImageApiMock { get; private set; } = imageApiMock;
        Database Database { get; set; } = default!;

        public async ValueTask InitializeAsync()
        {
            Database = await databaseFactory.Create();
        }

        public async ValueTask DisposeAsync()
        {
            await WebApplicationFactory.DisposeAsync();
            ImageApiMock?.Server.Dispose();
            await Database.DisposeAsync();
            InMemorySink.Dispose();
            InMemorySink.Instance.Dispose();
            await InjectableTestOutputSink.DisposeAsync();
            await Log.CloseAndFlushAsync();
        }
    }
}
