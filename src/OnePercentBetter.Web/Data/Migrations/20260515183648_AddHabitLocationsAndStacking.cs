using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHabitLocationsAndStacking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Habits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StackedAfterHabitId",
                table: "Habits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HabitLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitLocations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_LocationId",
                table: "Habits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_StackedAfterHabitId",
                table: "Habits",
                column: "StackedAfterHabitId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitLocations_UserId_Name",
                table: "HabitLocations",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_HabitLocations_LocationId",
                table: "Habits",
                column: "LocationId",
                principalTable: "HabitLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_Habits_StackedAfterHabitId",
                table: "Habits",
                column: "StackedAfterHabitId",
                principalTable: "Habits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_HabitLocations_LocationId",
                table: "Habits");

            migrationBuilder.DropForeignKey(
                name: "FK_Habits_Habits_StackedAfterHabitId",
                table: "Habits");

            migrationBuilder.DropTable(
                name: "HabitLocations");

            migrationBuilder.DropIndex(
                name: "IX_Habits_LocationId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_StackedAfterHabitId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "StackedAfterHabitId",
                table: "Habits");
        }
    }
}
