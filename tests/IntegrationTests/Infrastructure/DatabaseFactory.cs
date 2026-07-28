using Core;
using Persistence.Migrations;
using Docker.DotNet;
using Docker.DotNet.Models;
using IntegrationTests.Settings;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Testcontainers.PostgreSql;
using Xunit;
using Microsoft.Extensions.Hosting;

namespace IntegrationTests.Infrastructure
{
    internal class DatabaseFactory(ITestSettings testSettings)
    {
        static readonly SemaphoreSlim @lock = new(1, 1);
        const string DatabaseName = "Database";
        const string ContainerName = "TemplateApiIntegrationTestsPostgreSql";
        const int HostPort = 50000;

        public async Task<Database> Create()
        {
            await @lock.WaitAsync();

            try
            {
                WriteMessage("Initializing database.");

                WriteMessage("Using existing container if exists.");
                string connectionString;
                PostgreSqlContainer? container = default;
                if (await ExistsContainer())
                {
                    connectionString = GetConnectionString();
                }
                else
                {
                    WriteMessage("Does not exist. Creating new container.");
                    container = await CreateContainer();
                    WriteMessage("Container created.");
                    connectionString = GetConnectionString(container);
                }

                WriteMessage("Migrating database.");
                MigrateDatabase(connectionString);

                WriteMessage("Database initialized.");
                return new Database(testSettings, container) { ConnectionString = connectionString };
            }
            finally
            {
                @lock.Release();
            }
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
                    await client.Containers.StartContainerAsync(container!.ID, new ContainerStartParameters());

                return true;
            }

            return false;
        }

        async Task<PostgreSqlContainer> CreateContainer()
        {
            var postgreSqlContainer = new PostgreSqlBuilder("postgres:latest")
                .WithName(ContainerName!)
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
                Port = HostPort
            };

            return builder.ConnectionString;
        }

        static void MigrateDatabase(string connectionString)
        {
            Migrator.Migrate(connectionString);
        }

        static void WriteMessage(string message)
        {
            TestContext.Current.SendDiagnosticMessage(message);
        }
    }
}