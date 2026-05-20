namespace OnePercentBetter.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TodayCompletionRate { get; set; }

    public int CompletedToday { get; set; }

    public int FailedToday { get; set; }

    public int SkippedToday { get; set; }

    public int DueToday { get; set; }

    public int WeeklyCompletionRate { get; set; }

    public int CompletedLast7Days { get; set; }

    public int CheckInsLast7Days { get; set; }

    public int CurrentStreak { get; set; }

    public int BestStreak { get; set; }

    public int ActiveHabits { get; set; }

    public int ActiveGoals { get; set; }

    public int ActiveIdentities { get; set; }

    public int BetterIndex { get; set; }

    public int TodayTasksPending { get; set; }

    public int TodayTasksCompleted { get; set; }

    public int PendingTasks { get; set; }

    public int OverdueTasks { get; set; }

    public int CompletedTasksLast7Days { get; set; }

    public int UrgentTasks { get; set; }

    public string? NextTaskTitle { get; set; }

    public string? NextTaskTime { get; set; }

    public string? FocusIdentityName { get; set; }

    public string? FocusIdentityStatement { get; set; }

    public IReadOnlyList<TodayHabitViewModel> TodayHabits { get; set; } = [];

    public IReadOnlyList<WeeklyProgressPointViewModel> WeeklyProgress { get; set; } = [];

    public IReadOnlyList<string> Alerts { get; set; } = [];
}
