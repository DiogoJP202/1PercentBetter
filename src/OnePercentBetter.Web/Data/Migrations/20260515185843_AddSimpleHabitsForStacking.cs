using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSimpleHabitsForStacking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StackedAfterSimpleHabitId",
                table: "Habits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SimpleHabits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ScheduledTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleHabits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimpleHabits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_StackedAfterSimpleHabitId",
                table: "Habits",
                column: "StackedAfterSimpleHabitId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleHabits_UserId_IsActive",
                table: "SimpleHabits",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_SimpleHabits_UserId_Name_ScheduledTime",
                table: "SimpleHabits",
                columns: new[] { "UserId", "Name", "ScheduledTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_SimpleHabits_StackedAfterSimpleHabitId",
                table: "Habits",
                column: "StackedAfterSimpleHabitId",
                principalTable: "SimpleHabits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_SimpleHabits_StackedAfterSimpleHabitId",
                table: "Habits");

            migrationBuilder.DropTable(
                name: "SimpleHabits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_StackedAfterSimpleHabitId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "StackedAfterSimpleHabitId",
                table: "Habits");
        }
    }
}
