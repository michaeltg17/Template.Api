using DbUp;
using Persistence.Migrations.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Persistence.Migrations;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Used outside")]
public static class Migrator
{
    public static void Migrate(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .Build();

        upgrader.PerformUpgrade().ThrowOnError();
    }
}
