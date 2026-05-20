using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingTourTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OnboardingTourCompletedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnboardingTourSkippedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OnboardingTourVersion",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingTourCompletedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingTourSkippedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingTourVersion",
                table: "AspNetUsers");
        }
    }
}
