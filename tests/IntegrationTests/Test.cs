using IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Xunit;

namespace IntegrationTests
{
    public abstract class Test(TestFixture testFixture) : IAsyncLifetime
    {
        public const string ApiKey = "test-api-key";

        public ApiClient.ApiClient ApiClient { get; private set; } = default!;
        protected AppDbContext Context { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;
        protected HttpClient HttpClient { get; private set; } = default!;
        public TestFixture TestFixture { get; set; } = testFixture;

        public virtual ValueTask Initialize(string? collectionFixtureName)
        {
            TestFixture.InjectableTestOutputSink.Inject(TestContext.Current.TestOutputHelper!);
            TestFixture.ImageApiMock!.Server.ResetLogEntries();
            TestFixture.SetWebApplicationFactory(collectionFixtureName);

            ApiClient = new(TestFixture.WebApplicationFactory.CreateClient());
            Scope = TestFixture.WebApplicationFactory.Services.CreateAsyncScope();
            Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            HttpClient = Scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
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
            TestFixture.InMemorySink.Dispose();
            FlushLogger();
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// To be called at the end of each test so logs from previous test don't get mixed with the next one.
        /// </summary>
        public static void FlushLogger()
        {
            //Not the best but too hard to do it in another way.
            Thread.Sleep(10);
        }
    }
}
