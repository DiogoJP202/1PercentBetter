using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;

    public DateTime? TaskDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public int? IdentityId { get; set; }

    public UserIdentity? Identity { get; set; }

    public int? GoalId { get; set; }

    public Goal? Goal { get; set; }

    public int? HabitId { get; set; }

    public Habit? Habit { get; set; }

    [MaxLength(24)]
    public string Color { get; set; } = "#a78bfa";

    [MaxLength(80)]
    public string Icon { get; set; } = "list-checks";

    public bool ShowOnCalendar { get; set; } = true;

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<TaskItemTag> TaskItemTags { get; } = new List<TaskItemTag>();

    public ICollection<Note> NotesLinks { get; } = new List<Note>();
}
