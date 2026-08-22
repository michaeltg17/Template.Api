using Persistence.Migrations;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Npgsql;

namespace IntegrationTests.Infrastructure
{
    public class DatabaseFactory(ILogger logger, Migrator migrator)
    {
        const string DatabaseName = "template_db";

        public async Task<Database> Create(string? containerName = null, bool keepAlive = false)
        {
            logger.LogInformation("Creating database.");

            logger.LogInformation("Using existing container if exists.");
            string connectionString;
            PostgreSqlContainer? postgreSqlContainer = default;
            ContainerListResponse? container = await GetContainer(containerName);
            if (container != null)
            {
                connectionString = GetConnectionString(port: container.Ports[0].PublicPort);
            }
            else
            {
                logger.LogInformation("Does not exist. Creating new container.");
                postgreSqlContainer = await CreateContainer(keepAlive);
                logger.LogInformation("Container created.");
                connectionString = GetConnectionString(postgreSqlContainer);
            }

            logger.LogInformation("Migrating database.");
            migrator.Migrate(connectionString);

            logger.LogInformation("Database created.");
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
    }
}