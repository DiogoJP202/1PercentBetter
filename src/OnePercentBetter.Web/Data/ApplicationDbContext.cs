using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();

    public DbSet<Goal> Goals => Set<Goal>();

    public DbSet<Habit> Habits => Set<Habit>();

    public DbSet<SimpleHabit> SimpleHabits => Set<SimpleHabit>();

    public DbSet<HabitLocation> HabitLocations => Set<HabitLocation>();

    public DbSet<HabitLog> HabitLogs => Set<HabitLog>();

    public DbSet<DailyCheckIn> DailyCheckIns => Set<DailyCheckIn>();

    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.DisplayName).HasMaxLength(120);
            entity.Property(user => user.AvatarUrl).HasMaxLength(600);
            entity.Property(user => user.MainColor).HasMaxLength(24);
            entity.Property(user => user.ThemePreference).HasMaxLength(40);
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(category => new { category.UserId, category.Name });

            entity.HasOne(category => category.User)
                .WithMany(user => user.Categories)
                .HasForeignKey(category => category.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(
                DefaultCategory(1, "Tecnologia", "Code, projetos e aprendizado tecnico.", "#38bdf8", "code-2"),
                DefaultCategory(2, "Idiomas", "Estudos de linguas e pratica diaria.", "#a78bfa", "languages"),
                DefaultCategory(3, "Saude", "Corpo, sono, alimentacao e energia.", "#22c55e", "heart-pulse"),
                DefaultCategory(4, "Estudos", "Rotinas de aprendizado e revisao.", "#f59e0b", "book-open"),
                DefaultCategory(5, "Trabalho", "Carreira, entregas e foco profissional.", "#60a5fa", "briefcase-business"),
                DefaultCategory(6, "Projetos", "Projetos pessoais e entregas criativas.", "#34d399", "rocket"),
                DefaultCategory(7, "Financas", "Dinheiro, planejamento e controle.", "#84cc16", "wallet"),
                DefaultCategory(8, "Casa", "Organizacao domestica e rotina.", "#fb7185", "home"),
                DefaultCategory(9, "Social", "Relacionamentos e presenca.", "#f472b6", "users-round"),
                DefaultCategory(10, "Mental", "Foco, ansiedade e clareza.", "#818cf8", "brain"));
        });

        builder.Entity<UserIdentity>(entity =>
        {
            entity.ToTable("Identities");
            entity.HasIndex(identity => new { identity.UserId, identity.Status });
            entity.HasIndex(identity => new { identity.UserId, identity.Name });

            entity.HasOne(identity => identity.User)
                .WithMany(user => user.UserIdentities)
                .HasForeignKey(identity => identity.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(identity => identity.Category)
                .WithMany(category => category.UserIdentities)
                .HasForeignKey(identity => identity.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Goal>(entity =>
        {
            entity.HasIndex(goal => new { goal.UserId, goal.Status });
            entity.Property(goal => goal.StartDate).HasColumnType("date");
            entity.Property(goal => goal.TargetDate).HasColumnType("date");

            entity.HasOne(goal => goal.User)
                .WithMany(user => user.Goals)
                .HasForeignKey(goal => goal.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(goal => goal.Identity)
                .WithMany(identity => identity.Goals)
                .HasForeignKey(goal => goal.IdentityId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(goal => goal.Category)
                .WithMany(category => category.Goals)
                .HasForeignKey(goal => goal.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Habit>(entity =>
        {
            entity.HasIndex(habit => new { habit.UserId, habit.Status });

            entity.HasOne(habit => habit.User)
                .WithMany(user => user.Habits)
                .HasForeignKey(habit => habit.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(habit => habit.Identity)
                .WithMany(identity => identity.Habits)
                .HasForeignKey(habit => habit.IdentityId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(habit => habit.Goal)
                .WithMany(goal => goal.Habits)
                .HasForeignKey(habit => habit.GoalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(habit => habit.Category)
                .WithMany(category => category.Habits)
                .HasForeignKey(habit => habit.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(habit => habit.Location)
                .WithMany(location => location.Habits)
                .HasForeignKey(habit => habit.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(habit => habit.StackedAfterHabit)
                .WithMany(habit => habit.StackedHabits)
                .HasForeignKey(habit => habit.StackedAfterHabitId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(habit => habit.StackedAfterSimpleHabit)
                .WithMany(simpleHabit => simpleHabit.StackedHabits)
                .HasForeignKey(habit => habit.StackedAfterSimpleHabitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<HabitLocation>(entity =>
        {
            entity.HasIndex(location => new { location.UserId, location.Name }).IsUnique();

            entity.HasOne(location => location.User)
                .WithMany(user => user.HabitLocations)
                .HasForeignKey(location => location.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SimpleHabit>(entity =>
        {
            entity.HasIndex(simpleHabit => new { simpleHabit.UserId, simpleHabit.IsActive });
            entity.HasIndex(simpleHabit => new { simpleHabit.UserId, simpleHabit.Name, simpleHabit.ScheduledTime });

            entity.HasOne(simpleHabit => simpleHabit.User)
                .WithMany(user => user.SimpleHabits)
                .HasForeignKey(simpleHabit => simpleHabit.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<HabitLog>(entity =>
        {
            entity.Property(log => log.Date).HasColumnType("date");
            entity.HasIndex(log => new { log.UserId, log.HabitId, log.Date }).IsUnique();

            entity.HasOne(log => log.User)
                .WithMany()
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(log => log.Habit)
                .WithMany(habit => habit.Logs)
                .HasForeignKey(log => log.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<DailyCheckIn>(entity =>
        {
            entity.Property(checkIn => checkIn.Date).HasColumnType("date");
            entity.HasIndex(checkIn => new { checkIn.UserId, checkIn.Date }).IsUnique();

            entity.HasOne(checkIn => checkIn.User)
                .WithMany(user => user.DailyCheckIns)
                .HasForeignKey(checkIn => checkIn.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Note>(entity =>
        {
            entity.Property(note => note.Date).HasColumnType("date");
            entity.HasIndex(note => new { note.UserId, note.Date });

            entity.HasOne(note => note.User)
                .WithMany()
                .HasForeignKey(note => note.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(note => note.Identity)
                .WithMany(identity => identity.Notes)
                .HasForeignKey(note => note.IdentityId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(note => note.Goal)
                .WithMany(goal => goal.Notes)
                .HasForeignKey(note => note.GoalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(note => note.Habit)
                .WithMany(habit => habit.Notes)
                .HasForeignKey(note => note.HabitId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static Category DefaultCategory(int id, string name, string description, string color, string icon)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Description = description,
            Color = color,
            Icon = icon,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }
}
