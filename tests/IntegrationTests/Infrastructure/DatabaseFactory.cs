using Core;
using Persistence.Migrations;
using Docker.DotNet;
using Docker.DotNet.Models;
using IntegrationTests.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Testcontainers.MsSql;
using Xunit;
using Microsoft.Extensions.Hosting;

namespace IntegrationTests.Infrastructure
{
    internal class DatabaseFactory(ITestSettings testSettings)
    {
        static readonly SemaphoreSlim @lock = new(1, 1);
        const string DatabaseName = "Database";
        const string ContainerName = "IntegrationTestsSqlServer";
        const int HostPort = 50000;

        public async Task<Database> Create()
        {
            await @lock.WaitAsync();

            try
            {
                WriteMessage("Initializing database.");

                WriteMessage("Using existing container if exists.");
                string connectionString;
                MsSqlContainer? container = default;
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

        async Task<MsSqlContainer> CreateContainer()
        {
            var sqlServerContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                .WithName(ContainerName!)
                .WithPortBinding(HostPort, 1433)
                .WithCleanUp(!testSettings.KeepAliveDatabase)
                .WithAutoRemove(!testSettings.KeepAliveDatabase)
                .Build();

            await sqlServerContainer.StartAsync();

            return sqlServerContainer;
        }

        static string DockerHost => 
            Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE") ?? "localhost";

        static string GetConnectionString(MsSqlContainer? container = null)
        {
            if (DockerHost == "localhost" && container != null)
            {
                return container.GetConnectionString();
            }

            var builder = new SqlConnectionStringBuilder()
            {
                InitialCatalog = DatabaseName,
                UserID = MsSqlBuilder.DefaultUsername,
                Password = MsSqlBuilder.DefaultPassword,
                DataSource = DockerHost + "," + HostPort,
                TrustServerCertificate = true
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