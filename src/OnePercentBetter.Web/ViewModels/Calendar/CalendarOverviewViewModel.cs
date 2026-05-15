namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarOverviewViewModel
{
    public string MonthLabel { get; set; } = string.Empty;

    public int CompletedHabitLogs { get; set; }

    public int PlannedHabitOccurrences { get; set; }

    public int ConsistencyRate { get; set; }

    public int DaysWithCheckIn { get; set; }

    public int CurrentCheckInStreak { get; set; }

    public int ActiveSimpleHabits { get; set; }
}
