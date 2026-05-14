using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Habits;

public class HabitListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string TwoMinuteVersion { get; set; } = string.Empty;

    public string Trigger { get; set; } = string.Empty;

    public string? IdentityName { get; set; }

    public string? GoalTitle { get; set; }

    public string? CategoryName { get; set; }

    public ItemStatus Status { get; set; }

    public HabitLogStatus? TodayStatus { get; set; }

    public string Color { get; set; } = "#22c55e";

    public string Icon { get; set; } = "repeat-2";
}
