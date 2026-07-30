using Persistence.Migrations;
using Persistence.Migrations.Extensions;

string connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");
Migrator.Migrate(connectionString);