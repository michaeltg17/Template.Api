using Persistence.Migrations;
using Persistence.Migrations.Extensions;

string conn = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");
Migrator.Migrate(conn);