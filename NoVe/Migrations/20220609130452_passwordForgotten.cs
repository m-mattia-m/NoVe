using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoVe.Migrations
{
    public partial class passwordForgotten : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordForgottenValidFrom",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PasswordForgottenVerifyKey",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordForgottenValidFrom",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordForgottenVerifyKey",
                table: "Users");
        }
    }
}
