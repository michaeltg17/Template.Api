using Persistence.Migrations;

var connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");
ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
Migrator.Migrate(connectionString);