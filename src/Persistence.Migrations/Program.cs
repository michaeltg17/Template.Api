using Microsoft.Extensions.Logging;
using Persistence.Migrations;
using Serilog;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateLogger();

using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger, dispose: false));

try
{
    var connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
    new Migrator(loggerFactory).Migrate(connectionString);
}
finally
{
    Log.CloseAndFlush();
}
