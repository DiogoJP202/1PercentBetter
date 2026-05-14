using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class UserIdentity
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(260)]
    public string IdentityStatement { get; set; } = string.Empty;

    [MaxLength(800)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    [MaxLength(24)]
    public string Color { get; set; } = "#22c55e";

    [MaxLength(80)]
    public string Icon { get; set; } = "user-round-check";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Goal> Goals { get; } = new List<Goal>();

    public ICollection<Habit> Habits { get; } = new List<Habit>();

    public ICollection<Note> Notes { get; } = new List<Note>();
}
