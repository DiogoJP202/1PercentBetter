using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteForTasksGoalsIdentitiesHabits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_DueDate",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_Priority",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_Status",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_TaskDate",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_Identities_UserId_Name",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Identities_UserId_Status",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserId_Status",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Goals_UserId_Status",
                table: "Goals");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TaskItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TaskItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Identities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Identities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Habits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Habits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Goals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Goals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_IsDeleted_DueDate",
                table: "TaskItems",
                columns: new[] { "UserId", "IsDeleted", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_IsDeleted_Priority",
                table: "TaskItems",
                columns: new[] { "UserId", "IsDeleted", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_IsDeleted_Status",
                table: "TaskItems",
                columns: new[] { "UserId", "IsDeleted", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_IsDeleted_TaskDate",
                table: "TaskItems",
                columns: new[] { "UserId", "IsDeleted", "TaskDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UserId_IsDeleted_Name",
                table: "Identities",
                columns: new[] { "UserId", "IsDeleted", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UserId_IsDeleted_Status",
                table: "Identities",
                columns: new[] { "UserId", "IsDeleted", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId_IsDeleted_Status",
                table: "Habits",
                columns: new[] { "UserId", "IsDeleted", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId_IsDeleted_Status",
                table: "Goals",
                columns: new[] { "UserId", "IsDeleted", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_IsDeleted_DueDate",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_IsDeleted_Priority",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_IsDeleted_Status",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_UserId_IsDeleted_TaskDate",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_Identities_UserId_IsDeleted_Name",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Identities_UserId_IsDeleted_Status",
                table: "Identities");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserId_IsDeleted_Status",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Goals_UserId_IsDeleted_Status",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Identities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Identities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Goals");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_DueDate",
                table: "TaskItems",
                columns: new[] { "UserId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_Priority",
                table: "TaskItems",
                columns: new[] { "UserId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_Status",
                table: "TaskItems",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId_TaskDate",
                table: "TaskItems",
                columns: new[] { "UserId", "TaskDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UserId_Name",
                table: "Identities",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UserId_Status",
                table: "Identities",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId_Status",
                table: "Habits",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId_Status",
                table: "Goals",
                columns: new[] { "UserId", "Status" });
        }
    }
}
