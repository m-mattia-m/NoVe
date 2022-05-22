using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminVerification",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KlassenId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VarificationKey",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VarificationStatus",
                table: "User",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminVerification",
                table: "User");

            migrationBuilder.DropColumn(
                name: "KlassenId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VarificationKey",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VarificationStatus",
                table: "User");
        }
    }
}
