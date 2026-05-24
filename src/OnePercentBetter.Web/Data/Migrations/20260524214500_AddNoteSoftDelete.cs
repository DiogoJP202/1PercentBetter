using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId_Date",
                table: "Notes");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId_IsDeleted_Date",
                table: "Notes",
                columns: new[] { "UserId", "IsDeleted", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId_IsDeleted_Date",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId_Date",
                table: "Notes",
                columns: new[] { "UserId", "Date" });
        }
    }
}
