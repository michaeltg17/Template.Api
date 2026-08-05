using IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Xunit;

namespace IntegrationTests
{
    public abstract class Test : IAsyncLifetime
    {
        public const string ApiKey = "test-api-key";

        public ApiClient.ApiClient ApiClient { get; private set; } = default!;
        internal WebApplicationFactoryFixture WebApplicationFactoryFixture { get; set; } = default!;
        public ITestOutputHelper TestOutputHelper { get; set; } = default!;
        protected AppDbContext Context { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;
        protected HttpClient ImageHttpClient { get; private set; } = default!;

        public virtual ValueTask Initialize()
        {
            WebApplicationFactoryFixture.InjectableTestOutputSink.Inject(TestOutputHelper);
            ClearInMemorySink(WebApplicationFactoryFixture.InMemorySink);
            WebApplicationFactoryFixture.ImageApiMock!.Server.ResetLogEntries();
            ApiClient = new(WebApplicationFactoryFixture.CreateClient());

            Scope = WebApplicationFactoryFixture.Services.CreateAsyncScope();
            Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ImageHttpClient = Scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
            return ValueTask.CompletedTask;
        }

        Task<int> DeleteEntitiesFromDb()
        {
            var sql = "TRUNCATE TABLE products RESTART IDENTITY;";
            return Context.Database.ExecuteSqlRawAsync(sql);
        }

        public async ValueTask DisposeAsync()
        {
            await DeleteEntitiesFromDb();
            await Scope.DisposeAsync();
            WebApplicationFactoryFixture.FlushLogger();
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        static void ClearInMemorySink(Serilog.Sinks.InMemory.InMemorySink sink)
        {
            var field = typeof(Serilog.Sinks.InMemory.InMemorySink)
                .GetField("_logEvents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(sink, new System.Collections.Generic.List<Serilog.Events.LogEvent>());
        }
    }
}
