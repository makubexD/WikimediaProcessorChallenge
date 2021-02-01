using Microsoft.EntityFrameworkCore.Migrations;
using WikimediaProcessor.Data.MigrationHelpers;

namespace WikimediaProcessor.Data.Migrations
{
    public partial class StoredProceduresReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Down(migrationBuilder);

            var sql1 = MigrationHelper.GetSqlFromEmbeddedStoredProcedure("20210126-sp-language-domain-report.sql");
            migrationBuilder.Sql($"EXEC('{sql1}')");

            var sql2 = MigrationHelper.GetSqlFromEmbeddedStoredProcedure("20210126-sp-language-page-report.sql");
            migrationBuilder.Sql($"EXEC('{sql2}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS spLanguageDomainReport;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS spLanguagePageReport;");
        }
    }
}
