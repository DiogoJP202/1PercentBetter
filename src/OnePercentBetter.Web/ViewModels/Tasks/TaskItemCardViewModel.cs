using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskItemCardViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; }

    public TaskItemPriority Priority { get; set; }

    public DateTime? TaskDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public DateTime? DueDate { get; set; }

    public string? CategoryName { get; set; }

    public string? IdentityName { get; set; }

    public string? GoalTitle { get; set; }

    public string? HabitTitle { get; set; }

    public string Color { get; set; } = TaskVisualOptions.DefaultColor;

    public string Icon { get; set; } = TaskVisualOptions.DefaultIcon;

    public bool ShowOnCalendar { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsOverdue { get; set; }

    public IReadOnlyList<TaskTagBadgeViewModel> Tags { get; set; } = [];
}
