namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskItemDetailsViewModel
{
    public TaskItemCardViewModel Task { get; set; } = new();

    public IReadOnlyList<string> RelatedNotes { get; set; } = [];
}
