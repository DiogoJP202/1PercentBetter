using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class Habit
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public int? GoalId { get; set; }

    public Goal? Goal { get; set; }

    public int? IdentityId { get; set; }

    public UserIdentity? Identity { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public int? LocationId { get; set; }

    public HabitLocation? Location { get; set; }

    public int? StackedAfterHabitId { get; set; }

    public Habit? StackedAfterHabit { get; set; }

    public int? StackedAfterSimpleHabitId { get; set; }

    public SimpleHabit? StackedAfterSimpleHabit { get; set; }

    public HabitFrequencyType FrequencyType { get; set; } = HabitFrequencyType.Daily;

    [MaxLength(80)]
    public string? DaysOfWeek { get; set; }

    public TimeSpan? SuggestedTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public HabitDifficulty Difficulty { get; set; } = HabitDifficulty.Easy;

    [Required]
    [MaxLength(260)]
    public string TwoMinuteVersion { get; set; } = string.Empty;

    [Required]
    [MaxLength(260)]
    public string Trigger { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Reward { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    [MaxLength(24)]
    public string Color { get; set; } = "#22c55e";

    [MaxLength(80)]
    public string Icon { get; set; } = "repeat-2";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<HabitLog> Logs { get; } = new List<HabitLog>();

    public ICollection<TaskItem> TaskItems { get; } = new List<TaskItem>();

    public ICollection<Note> Notes { get; } = new List<Note>();

    public ICollection<Habit> StackedHabits { get; } = new List<Habit>();
}
