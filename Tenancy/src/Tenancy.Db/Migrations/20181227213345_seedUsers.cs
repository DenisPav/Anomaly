using Microsoft.EntityFrameworkCore.Migrations;

namespace Tenancy.Db.Migrations
{
    public partial class seedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "TenantId" },
                values: new object[,]
                {
                    { 1, "Bailee.Johns83@yahoo.com", 1 },
                    { 2, "Dejah72@yahoo.com", 2 },
                    { 3, "Florence46@yahoo.com", 1 },
                    { 4, "Lauren.Mertz8@hotmail.com", 2 },
                    { 5, "Donavon_Orn10@gmail.com", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
