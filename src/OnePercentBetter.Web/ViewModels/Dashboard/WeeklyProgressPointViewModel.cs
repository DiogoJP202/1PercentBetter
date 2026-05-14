namespace OnePercentBetter.Web.ViewModels.Dashboard;

public class WeeklyProgressPointViewModel
{
    public string Label { get; set; } = string.Empty;

    public int Completed { get; set; }

    public int Failed { get; set; }

    public int Skipped { get; set; }
}
