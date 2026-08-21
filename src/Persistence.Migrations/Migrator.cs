using DbUp;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;
using Persistence.Migrations.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Persistence.Migrations;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Used outside")]
public static class Migrator
{
    public static void Migrate(string connectionString, ILoggerFactory loggerFactory)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString, new MicrosoftUpgradeLog(loggerFactory));

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogTo(loggerFactory)
            .Build();

        upgrader.PerformUpgrade().ThrowOnError();
    }
}
