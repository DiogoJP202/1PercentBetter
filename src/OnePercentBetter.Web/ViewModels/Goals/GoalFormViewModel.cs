using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Goals;

public class GoalFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o titulo do objetivo.")]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200)]
    public string? Description { get; set; }

    public int? IdentityId { get; set; }

    public int? CategoryId { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    public GoalPriority Priority { get; set; } = GoalPriority.Medium;

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime? TargetDate { get; set; }

    [MaxLength(24)]
    public string Color { get; set; } = "#38bdf8";

    [MaxLength(80)]
    public string Icon { get; set; } = "target";

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];
}
