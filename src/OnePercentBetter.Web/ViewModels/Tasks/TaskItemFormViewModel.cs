using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskItemFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o titulo da tarefa.")]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;

    [DataType(DataType.Date)]
    public DateTime? TaskDate { get; set; } = AppClock.Today;

    [DataType(DataType.Time)]
    public TimeSpan? StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan? EndTime { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }

    public int? IdentityId { get; set; }

    public int? GoalId { get; set; }

    public int? HabitId { get; set; }

    [MaxLength(24)]
    [RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Escolha uma cor valida.")]
    public string Color { get; set; } = TaskVisualOptions.DefaultColor;

    [MaxLength(80)]
    public string Icon { get; set; } = TaskVisualOptions.DefaultIcon;

    public bool ShowOnCalendar { get; set; } = true;

    public List<int> SelectedTagIds { get; set; } = [];

    [MaxLength(240)]
    public string? NewTags { get; set; }

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Goals { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Habits { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Tags { get; set; } = [];

    public IReadOnlyList<TaskTagBadgeViewModel> TagItems { get; set; } = [];
}

