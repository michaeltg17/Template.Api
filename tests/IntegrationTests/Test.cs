using IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;
using Xunit;
using Xunit.v3;

namespace IntegrationTests
{
    public abstract class Test : IAsyncLifetime
    {
        public ApiClient.ApiClient ApiClient { get; private set; } = default!;
        internal WebApplicationFactoryFixture WebApplicationFactoryFixture { get; set; } = default!;
        public ITestOutputHelper TestOutputHelper { get; set; } = default!;
        protected AppDbContext Context { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;

        public virtual ValueTask Initialize()
        {
            WebApplicationFactoryFixture.InjectableTestOutputSink.Inject(TestOutputHelper);
            ClearInMemorySink(WebApplicationFactoryFixture.InMemorySink);
            ApiClient = new(WebApplicationFactoryFixture.CreateClient());

            Scope = WebApplicationFactoryFixture.Services.CreateAsyncScope();
            Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return ValueTask.CompletedTask;
        }

        Task<int> DeleteEntitiesFromDb()
        {
            var sql = "TRUNCATE TABLE Products;";
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
