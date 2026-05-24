using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Notes;

public class NoteFiltersViewModel
{
    public string View { get; set; } = "all";

    public string? Search { get; set; }

    public NoteType? NoteType { get; set; }

    public string? Tag { get; set; }

    public int? GoalId { get; set; }

    public int? IdentityId { get; set; }

    public int? HabitId { get; set; }

    public int? TaskItemId { get; set; }

    public string? Month { get; set; }

    public IReadOnlyList<SelectOptionViewModel> Tags { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Goals { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Habits { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> TaskItems { get; set; } = [];
}
