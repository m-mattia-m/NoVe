using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class updateSchreibfehlerInUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VarificationKey",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VarificationStatus",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "VerificationKey",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "User",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationKey",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "VarificationKey",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VarificationStatus",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
