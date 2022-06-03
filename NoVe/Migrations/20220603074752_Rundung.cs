using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class Rundung : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Rundung",
                table: "Kompetenzbereichs",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Rundung",
                table: "Fachs",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rundung",
                table: "Kompetenzbereichs");

            migrationBuilder.DropColumn(
                name: "Rundung",
                table: "Fachs");
        }
    }
}
