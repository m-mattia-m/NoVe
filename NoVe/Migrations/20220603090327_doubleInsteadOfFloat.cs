using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class doubleInsteadOfFloat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Rundung",
                table: "Kompetenzbereichs",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Rundung",
                table: "Fachs",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Rundung",
                table: "Kompetenzbereichs",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<float>(
                name: "Rundung",
                table: "Fachs",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
