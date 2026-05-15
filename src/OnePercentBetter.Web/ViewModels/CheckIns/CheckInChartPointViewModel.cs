namespace OnePercentBetter.Web.ViewModels.CheckIns;

public class CheckInChartPointViewModel
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public double Score { get; set; }

    public int Count { get; set; }

    public bool HasCheckIn { get; set; }

    public DateTime? Date { get; set; }

    public string Summary { get; set; } = string.Empty;
}
