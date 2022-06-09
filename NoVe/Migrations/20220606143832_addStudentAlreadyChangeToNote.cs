using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class addStudentAlreadyChangeToNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentAlreadyChanged",
                table: "Notes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentAlreadyChanged",
                table: "Notes");
        }
    }
}
