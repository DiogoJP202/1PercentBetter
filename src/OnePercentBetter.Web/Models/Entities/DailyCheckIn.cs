using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class DailyCheckIn
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public MoodLevel Mood { get; set; } = MoodLevel.Neutral;

    public int EnergyLevel { get; set; } = 3;

    public int ProductivityLevel { get; set; } = 3;

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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
