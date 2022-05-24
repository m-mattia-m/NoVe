using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Note",
                table: "Note");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kompetenzbereich",
                table: "Kompetenzbereich");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Klasse",
                table: "Klasse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fach",
                table: "Fach");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Beruf",
                table: "Beruf");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Note",
                newName: "Notes");

            migrationBuilder.RenameTable(
                name: "Kompetenzbereich",
                newName: "Kompetenzbereichs");

            migrationBuilder.RenameTable(
                name: "Klasse",
                newName: "Klasses");

            migrationBuilder.RenameTable(
                name: "Fach",
                newName: "Fachs");

            migrationBuilder.RenameTable(
                name: "Beruf",
                newName: "Berufs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notes",
                table: "Notes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kompetenzbereichs",
                table: "Kompetenzbereichs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Klasses",
                table: "Klasses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fachs",
                table: "Fachs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Berufs",
                table: "Berufs",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notes",
                table: "Notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kompetenzbereichs",
                table: "Kompetenzbereichs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Klasses",
                table: "Klasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fachs",
                table: "Fachs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Berufs",
                table: "Berufs");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "Note");

            migrationBuilder.RenameTable(
                name: "Kompetenzbereichs",
                newName: "Kompetenzbereich");

            migrationBuilder.RenameTable(
                name: "Klasses",
                newName: "Klasse");

            migrationBuilder.RenameTable(
                name: "Fachs",
                newName: "Fach");

            migrationBuilder.RenameTable(
                name: "Berufs",
                newName: "Beruf");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Note",
                table: "Note",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kompetenzbereich",
                table: "Kompetenzbereich",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Klasse",
                table: "Klasse",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fach",
                table: "Fach",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Beruf",
                table: "Beruf",
                column: "Id");
        }
    }
}
