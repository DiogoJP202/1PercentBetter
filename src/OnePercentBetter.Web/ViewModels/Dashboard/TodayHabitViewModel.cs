using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Dashboard;

public class TodayHabitViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string TwoMinuteVersion { get; set; } = string.Empty;

    public string Trigger { get; set; } = string.Empty;

    public TimeSpan? SuggestedTime { get; set; }

    public string Icon { get; set; } = "repeat-2";

    public string Color { get; set; } = "#22c55e";

    public HabitLogStatus? TodayStatus { get; set; }
}
