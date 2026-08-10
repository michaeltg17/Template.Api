using IntegrationTests.Collections;
using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace IntegrationTests.Fixtures
{
    public class TestFixture(
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock,
        IServiceProvider serviceProvider) : IAsyncLifetime
    {
        public WebApplicationFactory WebApplicationFactory { get; set; } = default!;
        public InMemorySink InMemorySink { get; } = inMemorySink;
        public InjectableTestOutputSink InjectableTestOutputSink { get; set; } = injectableTestOutputSink;
        internal ImageApiMock ImageApiMock { get; private set; } = imageApiMock;
        Database Database { get; set; } = default!;

        public async ValueTask InitializeAsync()
        {
            Database = await DatabaseFactory.Create();
        }

        [SuppressMessage("Style", "IDE0045:Convert to conditional expression", Justification = "Cleaner")]
        public void SetWebApplicationFactory(string? collectionFixtureName)
        {
            if (WebApplicationFactory != null) return;

            WebApplicationFactory webApplicationFactory;
            if (collectionFixtureName == nameof(DevelopmentApiCollectionFixture))
            {
                webApplicationFactory = new DevelopmentWebApplicationFactory(
                    serviceProvider.GetRequiredService<ITestSettings>(),
                    InMemorySink,
                    InjectableTestOutputSink,
                    ImageApiMock,
                    Database);
            }
            else if (collectionFixtureName == nameof(ProductionApiCollectionFixture))
            {
                webApplicationFactory = new ProductionWebApplicationFactory(
                    serviceProvider.GetRequiredService<ITestSettings>(),
                    InMemorySink,
                    InjectableTestOutputSink,
                    ImageApiMock,
                    Database);
            }
            else
            {
                throw new IntegrationTestsException(
                    $"Expected value '{collectionFixtureName}' to be development or production collection name.");
            }

            WebApplicationFactory = webApplicationFactory;
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
