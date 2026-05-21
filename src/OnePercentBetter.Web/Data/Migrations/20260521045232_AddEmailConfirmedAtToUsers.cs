using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailConfirmedAtToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmedAt",
                table: "AspNetUsers");
        }
    }
}
