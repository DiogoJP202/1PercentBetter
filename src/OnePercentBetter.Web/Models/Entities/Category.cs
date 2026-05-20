using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class Category
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(240)]
    public string? Description { get; set; }

    [MaxLength(24)]
    public string Color { get; set; } = "#22c55e";

    [MaxLength(80)]
    public string Icon { get; set; } = "sparkles";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserIdentity> UserIdentities { get; } = new List<UserIdentity>();

    public ICollection<Goal> Goals { get; } = new List<Goal>();

    public ICollection<Habit> Habits { get; } = new List<Habit>();

    public ICollection<TaskItem> TaskItems { get; } = new List<TaskItem>();
}
