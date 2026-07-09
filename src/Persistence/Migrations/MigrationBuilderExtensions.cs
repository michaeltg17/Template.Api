namespace Persistence.Migrations;

public static class MigrationBuilderExtensions
{
    public static string GetSql(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        var assembly = typeof(MigrationBuilderExtensions).Assembly;
        var resourceName = $"Persistence.Migrations.Sql.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource not found: {resourceName} (assembly: {assembly.FullName})");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}