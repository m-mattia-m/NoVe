using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class KlassenInvationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KlassenId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Firma",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KlasseId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LehrmeisterEmail",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KlassenInviteCode",
                table: "Klasses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllowedDomains = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_KlasseId",
                table: "Users",
                column: "KlasseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Klasses_KlasseId",
                table: "Users",
                column: "KlasseId",
                principalTable: "Klasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Klasses_KlasseId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Users_KlasseId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Firma",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KlasseId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LehrmeisterEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "KlassenInviteCode",
                table: "Klasses");

            migrationBuilder.AddColumn<int>(
                name: "KlassenId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
