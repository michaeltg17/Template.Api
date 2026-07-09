using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
        => migrationBuilder.Sql(MigrationBuilderExtensions.GetSql("Initial.up.sql"));

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.Sql(MigrationBuilderExtensions.GetSql("Initial.down.sql"));
}