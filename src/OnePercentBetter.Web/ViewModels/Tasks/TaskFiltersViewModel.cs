using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskFiltersViewModel
{
    public string View { get; set; } = "all";

    public string? Search { get; set; }

    public TaskItemPriority? Priority { get; set; }

    public int? TagId { get; set; }

    public int? GoalId { get; set; }

    public int? IdentityId { get; set; }

    public int? CategoryId { get; set; }

    public bool? WithTime { get; set; }

    public string GroupBy { get; set; } = "date";

    public IReadOnlyList<SelectOptionViewModel> Tags { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Goals { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];
}
