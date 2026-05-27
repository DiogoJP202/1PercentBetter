using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnePercentBetter.Web.ViewModels.CheckIns;

public class CheckInsOverviewViewModel
{
    public string Period { get; set; } = "month";

    public int Year { get; set; } = AppClock.Today.Year;

    public int Month { get; set; } = AppClock.Today.Month;

    public string PeriodLabel { get; set; } = string.Empty;

    public bool TodayHasCheckIn { get; set; }

    public CheckInMetricsViewModel Metrics { get; set; } = new();

    public CheckInDetailViewModel SelectedDetail { get; set; } = new();

    public IReadOnlyList<CheckInChartPointViewModel> Points { get; set; } = [];

    public IReadOnlyList<SelectListItem> MonthOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> YearOptions { get; set; } = [];
}

