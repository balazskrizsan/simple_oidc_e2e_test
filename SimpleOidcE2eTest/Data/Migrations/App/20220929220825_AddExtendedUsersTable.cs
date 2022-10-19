using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleOidcE2eTest.Migrations
{
    public partial class AddExtendedUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtendedUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    AccessToken = table.Column<string>(type: "text", nullable: true),
                    ProfileUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtendedUsers", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtendedUsers_ExternalId",
                table: "ExtendedUsers",
                column: "ExternalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtendedUsers");
        }
    }
}
