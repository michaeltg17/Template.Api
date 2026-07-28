using DbUp;
using Persistence.Migrations.Extensions;
using System.Reflection;

namespace Persistence.Migrations;

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
