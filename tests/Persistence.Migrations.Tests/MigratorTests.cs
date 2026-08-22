using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using Serilog.Sinks.XUnit.Injectable;
using Testcontainers.PostgreSql;
using Xunit;

namespace Persistence.Migrations.Tests;

public class MigratorTests
{
    static string DockerHost =>
        Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE") ?? "localhost";

    [Fact]
    public async Task Migrate_OnExistingDatabase_AppliesAllScripts_AndLogsExpectedMessages()
    {
        //Given
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var container = new PostgreSqlBuilder("postgres:18.6").WithDatabase("template_db").Build();
        await container.StartAsync(cancellationToken);
        var connectionString = container.GetConnectionString();

        using var inMemorySink = new InMemorySink();
        await using var injectableSink = new InjectableTestOutputSink();
        injectableSink.Inject(TestContext.Current.TestOutputHelper!);

        using var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink(inMemorySink)
            .WriteTo.Sink(injectableSink)
            .CreateLogger();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger, dispose: false));

        //When
        new Migrator(loggerFactory).Migrate(connectionString);

        //Then: database has been migrated (running AwesomeAssertions here loads the framework InMemorySink.Should() selects its adapter from)
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await AssertMigrationAppliedAsync(connection, cancellationToken);

        //Then: expected DbUp migration messages are logged at Information
        inMemorySink
            .Should()
            .HaveMessage("Beginning database upgrade")
            .Appearing().Times(1)
            .WithLevel(LogEventLevel.Information);
        inMemorySink
            .Should()
            .HaveMessage("Upgrade successful")
            .Appearing().Times(1)
            .WithLevel(LogEventLevel.Information);
    }

    [Fact]
    public async Task Migrate_OnMissingDatabase_CreatesDatabase_AndMigrates()
    {
        //Given: container is running but the target database does not exist yet
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var container = new PostgreSqlBuilder("postgres:18.6").Build();
        await container.StartAsync(cancellationToken);
        var connectionString = BuildConnectionString(container, "template_db");

        using var inMemorySink = new InMemorySink();
        await using var injectableSink = new InjectableTestOutputSink();
        injectableSink.Inject(TestContext.Current.TestOutputHelper!);

        using var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink(inMemorySink)
            .WriteTo.Sink(injectableSink)
            .CreateLogger();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger, dispose: false));

        //When
        new Migrator(loggerFactory).Migrate(connectionString);

        //Then: the missing database was created and migrated (also loads AwesomeAssertions for the sink adapter)
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await AssertMigrationAppliedAsync(connection, cancellationToken);

        //Then: migration succeeded
        inMemorySink
            .Should()
            .HaveMessage("Upgrade successful")
            .Appearing().Times(1)
            .WithLevel(LogEventLevel.Information);
    }

    static string BuildConnectionString(PostgreSqlContainer container, string database) =>
        new NpgsqlConnectionStringBuilder
        {
            Host = DockerHost,
            Port = container.GetMappedPublicPort(5432),
            Database = database,
            Username = PostgreSqlBuilder.DefaultUsername,
            Password = PostgreSqlBuilder.DefaultPassword,
            GssEncryptionMode = GssEncryptionMode.Disable,
        }.ConnectionString;

    static async Task AssertMigrationAppliedAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        List<string> scriptNames;
        using (var scriptsCommand = new NpgsqlCommand("SELECT \"scriptname\" FROM schemaversions;", connection))
        {
            var names = new List<string>();
            using var reader = await scriptsCommand.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                names.Add(reader.GetString(0));
            }
            scriptNames = names;
        }

        scriptNames.Should().BeEquivalentTo(
        [
            "Persistence.Migrations.Scripts.0001_Initial.sql",
            "Persistence.Migrations.Scripts.0002_AddImageName.sql",
            "Persistence.Migrations.Scripts.0003_RenameImageNameToFileName.sql",
        ]);

        const string columnQuery = "SELECT count(*) FROM information_schema.columns " +
            "WHERE table_schema = 'public' AND table_name = 'products' AND column_name = 'image_file_name';";
        using var columnCommand = new NpgsqlCommand(columnQuery, connection);
        var imageFileNameColumnCount = (long)(await columnCommand.ExecuteScalarAsync(cancellationToken))!;
        imageFileNameColumnCount.Should().Be(1);
    }
}
