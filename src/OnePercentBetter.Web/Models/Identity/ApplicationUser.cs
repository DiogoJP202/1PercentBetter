using Microsoft.AspNetCore.Identity;
using OnePercentBetter.Web.Models.Entities;

namespace OnePercentBetter.Web.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string MainColor { get; set; } = "#22c55e";

    public string ThemePreference { get; set; } = "dark";

    public DateTime? EmailConfirmedAt { get; set; }

    public DateTime? OnboardingTourCompletedAt { get; set; }

    public DateTime? OnboardingTourSkippedAt { get; set; }

    public int? OnboardingTourVersion { get; set; }

    public DateTime? OnboardingCompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Category> Categories { get; } = new List<Category>();

    public ICollection<UserIdentity> UserIdentities { get; } = new List<UserIdentity>();

    public ICollection<Goal> Goals { get; } = new List<Goal>();

    public ICollection<Habit> Habits { get; } = new List<Habit>();

    public ICollection<SimpleHabit> SimpleHabits { get; } = new List<SimpleHabit>();

    public ICollection<HabitLocation> HabitLocations { get; } = new List<HabitLocation>();

    public ICollection<DailyCheckIn> DailyCheckIns { get; } = new List<DailyCheckIn>();

    public ICollection<TaskItem> TaskItems { get; } = new List<TaskItem>();

    public ICollection<TaskTag> TaskTags { get; } = new List<TaskTag>();
}
