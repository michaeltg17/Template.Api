using IntegrationTests.Settings;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure
{
    public class Database(ITestSettings testSettings, PostgreSqlContainer? postgreSqlContainer) : IAsyncDisposable
    {
        public required string ConnectionString { get; init; }

        public ValueTask DisposeAsync()
        {
            if (testSettings.KeepAliveDatabase)
            {
                return ValueTask.CompletedTask;
            }

            GC.SuppressFinalize(this);
            return postgreSqlContainer?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}