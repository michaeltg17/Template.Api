using Persistence.Migrations;
using Docker.DotNet;
using Docker.DotNet.Models;
using IntegrationTests.Settings;
using Testcontainers.PostgreSql;
using Npgsql;
using Xunit;

namespace IntegrationTests.Infrastructure
{
    public class DatabaseFactory(ITestSettings testSettings)
    {
        const string DatabaseName = "database";
        const string ContainerName = "template-api-integration-tests-postgres";
        const int HostPort = 50000;

        public async Task<Database> Create()
        {
            Log("Initializing database.");

            Log("Using existing container if exists.");
            string connectionString;
            PostgreSqlContainer? container = default;
            if (await ExistsContainer())
            {
                connectionString = GetConnectionString();
            }
            else
            {
                Log("Does not exist. Creating new container.");
                container = await CreateContainer();
                Log("Container created.");
                connectionString = GetConnectionString(container);
            }

            Log("Migrating database.");
            Migrator.Migrate(connectionString);

            Log("Database initialized.");
            return new Database(testSettings, container) { ConnectionString = connectionString };
        }

        static async Task<bool> ExistsContainer()
        {
            var client = new DockerClientBuilder().Build();
            var parameters = new ContainersListParameters() { All = true };
            var containers = await client.Containers.ListContainersAsync(parameters);
            var container = containers.SingleOrDefault(c => c.Names.Contains("/" + ContainerName));
            if (container != null)
            {
                if (container.State != "running")
                    await client.Containers.StartContainerAsync(container.ID);

                return true;
            }

            return false;
        }

        async Task<PostgreSqlContainer> CreateContainer()
        {
            var postgreSqlContainer = new PostgreSqlBuilder("postgres:latest")
                .WithName(ContainerName)
                .WithPortBinding(HostPort, 5432)
                .WithCleanUp(!testSettings.KeepAliveDatabase)
                .WithAutoRemove(!testSettings.KeepAliveDatabase)
                .Build();

            await postgreSqlContainer.StartAsync();

            return postgreSqlContainer;
        }

        static string DockerHost => 
            Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE") ?? "localhost";

        static string GetConnectionString(PostgreSqlContainer? container = null)
        {
            if (DockerHost == "localhost" && container != null)
            {
                return container.GetConnectionString();
            }

            var builder = new NpgsqlConnectionStringBuilder()
            {
                Database = DatabaseName,
                Username = PostgreSqlBuilder.DefaultUsername,
                Password = PostgreSqlBuilder.DefaultPassword,
                Host = DockerHost,
                Port = HostPort,
                GssEncryptionMode = GssEncryptionMode.Disable
            };

            return builder.ConnectionString;
        }

        static void Log(string message)
        {
            TestContext.Current.SendDiagnosticMessage(message);
        }
    }
}