using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenancy.Db.Migrations
{
    public partial class seedTenants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Host" },
                values: new object[] { 1, "localhost:5000" });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Host" },
                values: new object[] { 2, "localhost:5001" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
