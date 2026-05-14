using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class HabitLog
{
    public int Id { get; set; }

    public int HabitId { get; set; }

    public Habit? Habit { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public HabitLogStatus Status { get; set; } = HabitLogStatus.Completed;

    public DateTime? CompletedAt { get; set; }

    public MoodLevel? Mood { get; set; }

    public int? EnergyLevel { get; set; }

    public int? DifficultyFelt { get; set; }

    [MaxLength(1200)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
