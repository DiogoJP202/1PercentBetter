using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Notes;

public class NoteFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o titulo.")]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escreva a anotacao.")]
    [MaxLength(6000)]
    public string Content { get; set; } = string.Empty;

    public NoteType NoteType { get; set; } = NoteType.DailyReflection;

    [MaxLength(500)]
    public string? Tags { get; set; }

    public int? GoalId { get; set; }

    public int? IdentityId { get; set; }

    public int? HabitId { get; set; }

    public int? TaskItemId { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    public IReadOnlyList<SelectOptionViewModel> Goals { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Habits { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> TaskItems { get; set; } = [];
}
