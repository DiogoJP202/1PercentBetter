namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarDayTaskViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Color { get; set; } = "#a78bfa";

    public string Icon { get; set; } = "list-checks";

    public string Status { get; set; } = "Pending";

    public string StatusLabel { get; set; } = "Pendente";

    public string StatusTone { get; set; } = "warning";

    public string Priority { get; set; } = "Medium";

    public string PriorityLabel { get; set; } = "Media";

    public string? TimeRange { get; set; }

    public string? GoalTitle { get; set; }

    public string? IdentityName { get; set; }

    public bool IsCompleted { get; set; }
}
