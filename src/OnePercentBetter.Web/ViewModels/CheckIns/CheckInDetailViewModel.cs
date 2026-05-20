using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.CheckIns;

public class CheckInDetailViewModel
{
    public DateTime Date { get; set; }

    public bool Exists { get; set; }

    public MoodLevel Mood { get; set; } = MoodLevel.Neutral;

    public int EnergyLevel { get; set; }

    public int ProductivityLevel { get; set; }

    public int DayScore { get; set; }

    public int TotalScore { get; set; }

    public string? SmallWin { get; set; }

    public string? MainDifficulty { get; set; }

    public string? TaskBlocker { get; set; }

    public string? TomorrowAdjustment { get; set; }

    public string? Notes { get; set; }

    public int PlannedTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int PostponedTasks { get; set; }

    public int PendingTasks { get; set; }
}
