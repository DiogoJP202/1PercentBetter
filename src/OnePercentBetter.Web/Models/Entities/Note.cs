using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class Note
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(6000)]
    public string Content { get; set; } = string.Empty;

    public NoteType NoteType { get; set; } = NoteType.DailyReflection;

    [MaxLength(500)]
    public string? Tags { get; set; }

    public int? GoalId { get; set; }

    public Goal? Goal { get; set; }

    public int? IdentityId { get; set; }

    public UserIdentity? Identity { get; set; }

    public int? HabitId { get; set; }

    public Habit? Habit { get; set; }

    public int? TaskItemId { get; set; }

    public TaskItem? TaskItem { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
}
