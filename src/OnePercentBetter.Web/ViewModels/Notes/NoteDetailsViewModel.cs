using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Notes;

public class NoteDetailsViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public NoteType NoteType { get; set; }

    public string? Tags { get; set; }

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? GoalTitle { get; set; }

    public string? IdentityName { get; set; }

    public string? HabitTitle { get; set; }

    public string? TaskTitle { get; set; }
}
