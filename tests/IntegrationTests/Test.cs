using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System.Collections;
using Xunit;

namespace IntegrationTests
{
    public abstract class Test : IAsyncLifetime
    {
        public const string ApiKey = "test-api-key";

        public ApiClient.ApiClient ApiClient { get; private set; } = default!;
        protected AppDbContext Context { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;
        protected HttpClient ImageHttpClient { get; private set; } = default!;
        public TestFixture TestFixture { get; set; }

        public virtual ValueTask Initialize(ITestOutputHelper testOutputHelper, Type fixtureType)
        {
            TestFixture.InjectableTestOutputSink.Inject(testOutputHelper);
            TestFixture.ImageApiMock!.Server.ResetLogEntries();

            var fixtureType = collectionName switch
            {
                nameof(DevelopmentApiCollectionFixture) => typeof(DevelopmentWebApplicationFactory),
                nameof(ProductionApiCollectionFixture) => typeof(ProductionWebApplicationFactory),
                _ => throw new IntegrationTestsException("Expected development or production collection name.")
            };

            TestFixture.WebApplicationFactory = new 

            ApiClient = new(TestFixture.WebApplicationFactory.CreateClient());
            Scope = TestFixture.WebApplicationFactory.Services.CreateAsyncScope();
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
            FlushLogger();
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// To be called at the end of each test so that logs from previous test doesn't get mixed with the next one.
        /// </summary>
        public static void FlushLogger()
        {
            //Not the best but too hard to do it in another way.
            Thread.Sleep(10);
        }
    }
}
