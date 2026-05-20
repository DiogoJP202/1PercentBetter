using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.CheckIns;

public class DailyCheckInViewModel
{
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    public MoodLevel Mood { get; set; } = MoodLevel.Neutral;

    [Range(1, 5, ErrorMessage = "Informe uma nota entre 1 e 5.")]
    public int EnergyLevel { get; set; } = 3;

    [Range(1, 5, ErrorMessage = "Informe uma nota entre 1 e 5.")]
    public int ProductivityLevel { get; set; } = 3;

    [Range(1, 5, ErrorMessage = "Informe uma nota entre 1 e 5.")]
    public int DayScore { get; set; } = 3;

    [MaxLength(500)]
    public string? SmallWin { get; set; }

    [MaxLength(500)]
    public string? MainDifficulty { get; set; }

    [MaxLength(500)]
    public string? TaskBlocker { get; set; }

    [MaxLength(500)]
    public string? TomorrowAdjustment { get; set; }

    [MaxLength(1600)]
    public string? Notes { get; set; }

    public int PlannedTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int PostponedTasks { get; set; }

    public int PendingTasks { get; set; }
}
