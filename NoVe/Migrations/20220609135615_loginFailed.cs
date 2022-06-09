using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class loginFailed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoginFailedCount",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginFailedFrom",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginFailedCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginFailedFrom",
                table: "Users");
        }
    }
}
