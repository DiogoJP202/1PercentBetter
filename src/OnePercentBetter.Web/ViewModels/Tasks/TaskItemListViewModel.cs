namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskItemListViewModel
{
    public TaskFiltersViewModel Filters { get; set; } = new();

    public IReadOnlyList<TaskItemCardViewModel> Items { get; set; } = [];

    public IReadOnlyList<TaskItemCardViewModel> TodayItems { get; set; } = [];

    public IReadOnlyList<TaskItemCardViewModel> FutureItems { get; set; } = [];

    public IReadOnlyList<TaskItemCardViewModel> OverdueItems { get; set; } = [];

    public IReadOnlyList<TaskItemCardViewModel> CompletedItems { get; set; } = [];

    public int TodayCount { get; set; }

    public int PendingCount { get; set; }

    public int OverdueCount { get; set; }

    public int CompletedCount { get; set; }
}
