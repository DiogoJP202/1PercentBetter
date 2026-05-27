using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Goals;

public class GoalFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o tÃ­tulo do objetivo.")]
    [MaxLength(160, ErrorMessage = "O tÃ­tulo deve ter no mÃ¡ximo 160 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1200, ErrorMessage = "A descriÃ§Ã£o deve ter no mÃ¡ximo 1200 caracteres.")]
    public string? Description { get; set; }

    public int? IdentityId { get; set; }

    public int? CategoryId { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    public GoalPriority Priority { get; set; } = GoalPriority.Medium;

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = AppClock.Today;

    [DataType(DataType.Date)]
    public DateTime? TargetDate { get; set; }

    [MaxLength(24, ErrorMessage = "A cor deve ter no mÃ¡ximo 24 caracteres.")]
    public string Color { get; set; } = "#38bdf8";

    [MaxLength(80, ErrorMessage = "O Ã­cone deve ter no mÃ¡ximo 80 caracteres.")]
    public string Icon { get; set; } = "target";

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];

    public IReadOnlyList<SelectOptionViewModel> Identities { get; set; } = [];
}

