using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Models.Entities;

public class TaskTag
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(24)]
    public string Color { get; set; } = "#8b5cf6";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TaskItemTag> TaskItemTags { get; } = new List<TaskItemTag>();
}
