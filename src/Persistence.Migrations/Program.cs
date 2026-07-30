using Persistence.Migrations;

string connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");
Migrator.Migrate(connectionString);