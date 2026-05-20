using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class Goal
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public int? IdentityId { get; set; }

    public UserIdentity? Identity { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    public GoalPriority Priority { get; set; } = GoalPriority.Medium;

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime? TargetDate { get; set; }

    [MaxLength(24)]
    public string Color { get; set; } = "#38bdf8";

    [MaxLength(80)]
    public string Icon { get; set; } = "target";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Habit> Habits { get; } = new List<Habit>();

    public ICollection<TaskItem> TaskItems { get; } = new List<TaskItem>();

    public ICollection<Note> Notes { get; } = new List<Note>();
}
