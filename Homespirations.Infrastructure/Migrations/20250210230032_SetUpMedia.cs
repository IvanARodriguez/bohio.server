using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homespirations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetUpMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "HomeSpaces",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "HomeSpaces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "HomeSpaces",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "HomeSpaces",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "HomeSpaces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "HomeSpaces",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "HomeSpaces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "HomeSpaces",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HomeSpaceId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_HomeSpaces_HomeSpaceId",
                        column: x => x.HomeSpaceId,
                        principalTable: "HomeSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeSpaces_OwnerId",
                table: "HomeSpaces",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeSpaces_Status",
                table: "HomeSpaces",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Media_HomeSpaceId",
                table: "Media",
                column: "HomeSpaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropIndex(
                name: "IX_HomeSpaces_OwnerId",
                table: "HomeSpaces");

            migrationBuilder.DropIndex(
                name: "IX_HomeSpaces_Status",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "City",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "State",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "HomeSpaces");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "HomeSpaces");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "HomeSpaces",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
