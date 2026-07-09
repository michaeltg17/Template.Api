using Core;
using Docker.DotNet;
using Docker.DotNet.Models;
using IntegrationTests.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Testcontainers.MsSql;
using Xunit.Sdk;
using Xunit.v3;

namespace IntegrationTests.Infrastructure
{
    internal class DatabaseFactory(IMessageSink messageSink, ITestSettings testSettings) : IFactory<Database>
    {
        const string DatabaseName = "Database";
        const string ContainerName = "IntegrationTestsSqlServer";
        const int HostPort = 50000;

        public async Task<Database> Create()
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

            WriteMessage("Running migrations.");
            await RunMigrations(connectionString);

            WriteMessage("Database initialized.");
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
                    await client.Containers.StartContainerAsync(container!.ID, new ContainerStartParameters());

                return true;
            }

            return false;
        }

        async Task<MsSqlContainer> CreateContainer()
        {
            var sqlServerContainer = new MsSqlBuilder()
                .WithName(ContainerName)
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPortBinding(HostPort, 1433)
                .WithCleanUp(!testSettings.KeepAliveDatabase)
                .WithAutoRemove(!testSettings.KeepAliveDatabase)
                .Build();

            await sqlServerContainer.StartAsync();

            return sqlServerContainer;
        }

        static string GetConnectionString(MsSqlContainer container)
        {
            var builder = new SqlConnectionStringBuilder(container.GetConnectionString())
            {
                InitialCatalog = DatabaseName
            };

            return builder.ConnectionString;
        }

        static string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder()
            {
                InitialCatalog = DatabaseName,
                UserID = MsSqlBuilder.DefaultUsername,
                Password = MsSqlBuilder.DefaultPassword,
                DataSource = "localhost," + HostPort,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }

        static async Task RunMigrations(string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
                .Options;

            using var context = new AppDbContext(options);
            await context.Database.MigrateAsync();
        }

        void WriteMessage(string message) => messageSink.OnMessage(new DiagnosticMessage(message));
    }
}