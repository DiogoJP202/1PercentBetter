using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class HabitLocation
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Habit> Habits { get; } = new List<Habit>();
}
