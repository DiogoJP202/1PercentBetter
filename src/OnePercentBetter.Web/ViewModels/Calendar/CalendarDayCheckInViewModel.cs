namespace OnePercentBetter.Web.ViewModels.Calendar;

public class CalendarDayCheckInViewModel
{
    public int Id { get; set; }

    public string MoodLabel { get; set; } = "Neutro";

    public string MoodFace { get; set; } = "😐";

    public int TotalScore { get; set; }

    public string? SmallWin { get; set; }

    public string? MainDifficulty { get; set; }
}
