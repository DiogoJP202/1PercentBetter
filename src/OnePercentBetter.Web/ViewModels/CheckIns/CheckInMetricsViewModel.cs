namespace OnePercentBetter.Web.ViewModels.CheckIns;

public class CheckInMetricsViewModel
{
    public int CheckInCount { get; set; }

    public double AverageScore { get; set; }

    public int? BestScore { get; set; }

    public DateTime? BestDate { get; set; }

    public int CurrentStreak { get; set; }
}
