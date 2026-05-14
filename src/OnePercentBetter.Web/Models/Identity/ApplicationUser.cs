using Microsoft.AspNetCore.Identity;
using OnePercentBetter.Web.Models.Entities;

namespace OnePercentBetter.Web.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string MainColor { get; set; } = "#22c55e";

    public string ThemePreference { get; set; } = "dark";

    public DateTime? OnboardingCompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Category> Categories { get; } = new List<Category>();

    public ICollection<UserIdentity> UserIdentities { get; } = new List<UserIdentity>();

    public ICollection<Goal> Goals { get; } = new List<Goal>();

    public ICollection<Habit> Habits { get; } = new List<Habit>();

    public ICollection<DailyCheckIn> DailyCheckIns { get; } = new List<DailyCheckIn>();
}
