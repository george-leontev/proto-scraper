using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scraper.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Article",
                schema: "Business",
                table: "Product",
                newName: "Uid");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Business",
                table: "Product",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckingDate",
                schema: "Business",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckingDate",
                schema: "Business",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Uid",
                schema: "Business",
                table: "Product",
                newName: "Article");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Business",
                table: "Product",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);
        }
    }
}
