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
        AppDbContext DbContext { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;

        public void Initialize()
        {
            WebApplicationFactoryFixture.InjectableTestOutputSink.Inject(TestOutputHelper);
            WebApplicationFactoryFixture.InMemorySink = new InMemorySink();
            ApiClient = new(WebApplicationFactoryFixture.CreateClient());

            Scope = WebApplicationFactoryFixture.Services.CreateAsyncScope();
            DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        Task<int> DeleteEntitiesFromDb()
        {
            var sql =
                "DELETE FROM Images;" +
                "DELETE FROM ImageGroups;";
            return DbContext.Database.ExecuteSqlRawAsync(sql);
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
    }
}
