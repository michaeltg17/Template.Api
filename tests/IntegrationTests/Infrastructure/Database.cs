using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure
{
    public class Database(PostgreSqlContainer? postgreSqlContainer, bool keepAlive = false) : IAsyncDisposable
    {
        public required string ConnectionString { get; init; }

        public ValueTask DisposeAsync()
        {
            if (keepAlive)
            {
                return ValueTask.CompletedTask;
            }

            GC.SuppressFinalize(this);
            return postgreSqlContainer?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}