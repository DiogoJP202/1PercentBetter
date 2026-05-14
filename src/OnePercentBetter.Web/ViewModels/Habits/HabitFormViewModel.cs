using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Habits;

public class HabitFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o titulo do habito.")]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    public int? IdentityId { get; set; }

    public int? GoalId { get; set; }

    public int? CategoryId { get; set; }

    public HabitFrequencyType FrequencyType { get; set; } = HabitFrequencyType.Daily;

    [MaxLength(80)]
    public string? DaysOfWeek { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan? SuggestedTime { get; set; }

    public HabitDifficulty Difficulty { get; set; } = HabitDifficulty.Easy;

    [Required(ErrorMessage = "Defina a versao de 2 minutos.")]
    [MaxLength(260)]
    public string TwoMinuteVersion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Defina o gatilho do habito.")]
    [MaxLength(260)]
    public string Trigger { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Reward { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    [MaxLength(24)]
    public string Color { get; set; } = "#22c55e";

    [MaxLength(80)]
    public string Icon { get; set; } = "repeat-2";

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Goals { get; set; } = [];
}
