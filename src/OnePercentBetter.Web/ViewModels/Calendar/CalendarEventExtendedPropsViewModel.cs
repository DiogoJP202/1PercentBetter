namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarEventExtendedPropsViewModel
{
    public string Type { get; set; } = string.Empty;

    public string TypeLabel { get; set; } = string.Empty;

    public int? HabitId { get; set; }

    public int? SimpleHabitId { get; set; }

    public int? CheckInId { get; set; }

    public string Date { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string StatusLabel { get; set; } = string.Empty;

    public string HabitColor { get; set; } = "#34d399";

    public string HabitIcon { get; set; } = "repeat-2";

    public string? Time { get; set; }

    public string? Notes { get; set; }
}
