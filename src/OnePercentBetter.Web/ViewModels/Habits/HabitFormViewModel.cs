using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Habits;

public class HabitFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o título do hábito.")]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    public int? IdentityId { get; set; }

    public int? GoalId { get; set; }

    public int? CategoryId { get; set; }

    public int? LocationId { get; set; }

    public int? StackedAfterHabitId { get; set; }

    public int? StackedAfterSimpleHabitId { get; set; }

    public string? StackBaseKey { get; set; }

    public HabitFrequencyType FrequencyType { get; set; } = HabitFrequencyType.Daily;

    [MaxLength(80)]
    public string? DaysOfWeek { get; set; }

    public List<DayOfWeek> SelectedDaysOfWeek { get; set; } = [];

    [DataType(DataType.Time)]
    public TimeSpan? SuggestedTime { get; set; }

    public HabitDifficulty Difficulty { get; set; } = HabitDifficulty.Easy;

    [Required(ErrorMessage = "Defina a versão de 2 minutos.")]
    [MaxLength(260)]
    public string TwoMinuteVersion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Defina o gatilho do hábito.")]
    [MaxLength(260)]
    public string Trigger { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Reward { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    [MaxLength(24)]
    [RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Escolha uma cor válida.")]
    public string Color { get; set; } = HabitVisualOptions.DefaultColor;

    [MaxLength(80)]
    public string Icon { get; set; } = HabitVisualOptions.DefaultIcon;

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Goals { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Locations { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> StackableHabits { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> SimpleHabits { get; set; } = [];
}
