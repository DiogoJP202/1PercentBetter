using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnePercentBetter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateCoreHabitSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "AspNetUsers",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainColor",
                table: "AspNetUsers",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OnboardingCompletedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThemePreference",
                table: "AspNetUsers",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DailyCheckIns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Mood = table.Column<int>(type: "int", nullable: false),
                    EnergyLevel = table.Column<int>(type: "int", nullable: false),
                    ProductivityLevel = table.Column<int>(type: "int", nullable: false),
                    DayScore = table.Column<int>(type: "int", nullable: false),
                    SmallWin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MainDifficulty = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TomorrowAdjustment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1600)", maxLength: 1600, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyCheckIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyCheckIns_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Identities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IdentityStatement = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Identities_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdentityId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "date", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Goals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Goals_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GoalId = table.Column<int>(type: "int", nullable: true),
                    IdentityId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    FrequencyType = table.Column<int>(type: "int", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SuggestedTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    TwoMinuteVersion = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Trigger = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Reward = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Habits_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Habits_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Habits_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HabitLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mood = table.Column<int>(type: "int", nullable: true),
                    EnergyLevel = table.Column<int>(type: "int", nullable: true),
                    DifficultyFelt = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HabitLogs_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 6000, nullable: false),
                    NoteType = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GoalId = table.Column<int>(type: "int", nullable: true),
                    IdentityId = table.Column<int>(type: "int", nullable: true),
                    HabitId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notes_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notes_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notes_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Icon", "Name", "UserId" },
                values: new object[,]
                {
                    { 1, "#38bdf8", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Code, projetos e aprendizado tecnico.", "code-2", "Tecnologia", null },
                    { 2, "#a78bfa", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Estudos de linguas e pratica diaria.", "languages", "Idiomas", null },
                    { 3, "#22c55e", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Corpo, sono, alimentacao e energia.", "heart-pulse", "Saude", null },
                    { 4, "#f59e0b", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rotinas de aprendizado e revisao.", "book-open", "Estudos", null },
                    { 5, "#60a5fa", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Carreira, entregas e foco profissional.", "briefcase-business", "Trabalho", null },
                    { 6, "#34d399", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Projetos pessoais e entregas criativas.", "rocket", "Projetos", null },
                    { 7, "#84cc16", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dinheiro, planejamento e controle.", "wallet", "Financas", null },
                    { 8, "#fb7185", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Organizacao domestica e rotina.", "home", "Casa", null },
                    { 9, "#f472b6", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Relacionamentos e presenca.", "users-round", "Social", null },
                    { 10, "#818cf8", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Foco, ansiedade e clareza.", "brain", "Mental", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId_Name",
                table: "Categories",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyCheckIns_UserId_Date",
                table: "DailyCheckIns",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_CategoryId",
                table: "Goals",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_IdentityId",
                table: "Goals",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId_Status",
                table: "Goals",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HabitLogs_HabitId",
                table: "HabitLogs",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitLogs_UserId_HabitId_Date",
                table: "HabitLogs",
                columns: new[] { "UserId", "HabitId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_CategoryId",
                table: "Habits",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_GoalId",
                table: "Habits",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_IdentityId",
                table: "Habits",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId_Status",
                table: "Habits",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_CategoryId",
                table: "Identities",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UserId_Name",
                table: "Identities",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Identities_UserId_Status",
                table: "Identities",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_GoalId",
                table: "Notes",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_HabitId",
                table: "Notes",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_IdentityId",
                table: "Notes",
                column: "IdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId_Date",
                table: "Notes",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyCheckIns");

            migrationBuilder.DropTable(
                name: "HabitLogs");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Identities");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MainColor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ThemePreference",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetUsers");
        }
    }
}
