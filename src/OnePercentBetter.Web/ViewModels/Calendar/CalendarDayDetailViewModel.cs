namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarDayDetailViewModel
{
    public DateTime Date { get; set; }

    public string DateLabel { get; set; } = string.Empty;

    public int PlannedCount { get; set; }

    public int CompletedCount { get; set; }

    public int PendingCount { get; set; }

    public int PlannedTasksCount { get; set; }

    public int CompletedTasksCount { get; set; }

    public int PendingTasksCount { get; set; }

    public IReadOnlyList<CalendarDayHabitViewModel> ImprovementHabits { get; set; } = [];

    public IReadOnlyList<CalendarDaySimpleHabitViewModel> CommonHabits { get; set; } = [];

    public IReadOnlyList<CalendarDayTaskViewModel> Tasks { get; set; } = [];

    public CalendarDayCheckInViewModel? CheckIn { get; set; }
}
