using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.ViewModels.Notes;

public class NoteListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ContentPreview { get; set; } = string.Empty;

    public NoteType NoteType { get; set; }

    public string? Tags { get; set; }

    public DateTime Date { get; set; }
}
