using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Goals;

public class GoalListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? IdentityName { get; set; }

    public string? CategoryName { get; set; }

    public ItemStatus Status { get; set; }

    public GoalPriority Priority { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? TargetDate { get; set; }

    public string Color { get; set; } = "#38bdf8";

    public string Icon { get; set; } = "target";

    public int HabitsCount { get; set; }
}
