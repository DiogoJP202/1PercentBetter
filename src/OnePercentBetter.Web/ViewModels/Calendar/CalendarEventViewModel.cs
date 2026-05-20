namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarEventViewModel
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Start { get; set; } = string.Empty;

    public string? End { get; set; }

    public bool AllDay { get; set; } = true;

    public string BackgroundColor { get; set; } = "#34d399";

    public string BorderColor { get; set; } = "#34d399";

    public string TextColor { get; set; } = "#020617";

    public IReadOnlyList<string> ClassNames { get; set; } = [];

    public CalendarEventExtendedPropsViewModel ExtendedProps { get; set; } = new();
}
