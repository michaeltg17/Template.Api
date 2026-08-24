using Persistence.Migrations;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Npgsql;

namespace IntegrationTests.Infrastructure
{
    public partial class DatabaseFactory(ILogger<DatabaseFactory> logger, Migrator migrator)
    {
        const string DatabaseName = "template_db";

        public async Task<Database> Create(string? containerName = null, bool keepAlive = false)
        {
            LogCreatingDatabase();

            LogUsingExistingContainer();
            string connectionString;
            PostgreSqlContainer? postgreSqlContainer = default;
            ContainerListResponse? container = await GetContainer(containerName);
            if (container != null)
            {
                connectionString = GetConnectionString(port: container.Ports[0].PublicPort);
            }
            else
            {
                LogDoesNotExistCreatingNewContainer();
                postgreSqlContainer = await CreateContainer(keepAlive);
                LogContainerCreated();
                connectionString = GetConnectionString(postgreSqlContainer);
            }

            LogMigratingDatabase();
            migrator.Migrate(connectionString);

            LogDatabaseCreated();
            return new Database(postgreSqlContainer, keepAlive) { ConnectionString = connectionString };
        }

        static async Task<ContainerListResponse?> GetContainer(string? containerName)
        {
            if (containerName == null) return null;
            var client = new DockerClientBuilder().Build();
            var parameters = new ContainersListParameters() { All = true };
            var containers = await client.Containers.ListContainersAsync(parameters);
            var container = containers.SingleOrDefault(c => c.Names.Contains("/" + containerName));
            if (container != null)
            {
                if (container.State != "running")
                    await client.Containers.StartContainerAsync(container.ID);

                return container;
            }

            return container;
        }

        static async Task<PostgreSqlContainer> CreateContainer(bool keepAlive)
        {
            var postgreSqlContainer = new PostgreSqlBuilder("postgres:18.6")
                .WithDatabase(DatabaseName)
                .WithCleanUp(!keepAlive)
                .WithAutoRemove(!keepAlive)
                .Build();

            await postgreSqlContainer.StartAsync();

            return postgreSqlContainer;
        }

        static string DockerHost => 
            Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE") ?? "localhost";

        static string GetConnectionString(PostgreSqlContainer? container = null, int? port = null)
        {
            if (DockerHost == "localhost" && container != null)
            {
                return container.GetConnectionString();
            }

            if (!port.HasValue)
            {
                throw new ArgumentNullException(nameof(port));
            }

            var builder = new NpgsqlConnectionStringBuilder()
            {
                Database = DatabaseName,
                Username = PostgreSqlBuilder.DefaultUsername,
                Password = PostgreSqlBuilder.DefaultPassword,
                Host = DockerHost,
                Port = port.Value,
                GssEncryptionMode = GssEncryptionMode.Disable
            };

            return builder.ConnectionString;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Creating database.")]
        public partial void LogCreatingDatabase();

        [LoggerMessage(Level = LogLevel.Information, Message = "Using existing container if exists.")]
        public partial void LogUsingExistingContainer();

        [LoggerMessage(Level = LogLevel.Information, Message = "Does not exist. Creating new container.")]
        public partial void LogDoesNotExistCreatingNewContainer();

        [LoggerMessage(Level = LogLevel.Information, Message = "Container created.")]
        public partial void LogContainerCreated();

        [LoggerMessage(Level = LogLevel.Information, Message = "Migrating database.")]
        public partial void LogMigratingDatabase();

        [LoggerMessage(Level = LogLevel.Information, Message = "Database created.")]
        public partial void LogDatabaseCreated();
    }
}