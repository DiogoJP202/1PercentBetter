namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarDayHabitViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Color { get; set; } = "#22c55e";

    public string Icon { get; set; } = "repeat-2";

    public string? SuggestedTime { get; set; }

    public string? IdentityName { get; set; }

    public string? GoalTitle { get; set; }

    public string? LocationName { get; set; }

    public string Status { get; set; } = "Pending";

    public string StatusLabel { get; set; } = "Pendente";

    public string StatusTone { get; set; } = "warning";

    public bool IsCompleted { get; set; }
}
