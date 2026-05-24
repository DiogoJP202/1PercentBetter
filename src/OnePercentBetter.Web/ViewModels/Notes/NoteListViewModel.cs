namespace OnePercentBetter.Web.ViewModels.Notes;

public class NoteListViewModel
{
    public NoteFiltersViewModel Filters { get; set; } = new();

    public IReadOnlyList<NoteListItemViewModel> Items { get; set; } = [];

    public IReadOnlyList<NoteListItemViewModel> MonthItems { get; set; } = [];

    public int TotalCount { get; set; }

    public int MonthCount { get; set; }

    public int NotesWithTagsCount { get; set; }

    public int ReviewsCount { get; set; }

    public string MonthLabel { get; set; } = string.Empty;
}
