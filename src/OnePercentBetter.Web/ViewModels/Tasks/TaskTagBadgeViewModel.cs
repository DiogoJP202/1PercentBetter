namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskTagBadgeViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = TaskVisualOptions.DefaultColor;
}
