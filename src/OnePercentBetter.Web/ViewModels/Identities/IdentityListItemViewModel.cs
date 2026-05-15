using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Identities;

public class IdentityListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string IdentityStatement { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? CategoryName { get; set; }

    public ItemStatus Status { get; set; }

    public string Color { get; set; } = "#22c55e";

    public string Icon { get; set; } = "user-round-check";

    public int HabitsCount { get; set; }

    public int GoalsCount { get; set; }
}
