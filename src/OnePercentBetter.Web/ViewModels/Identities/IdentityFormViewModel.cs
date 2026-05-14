using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Identities;

public class IdentityFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da identidade.")]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escreva a frase da identidade.")]
    [MaxLength(260)]
    public string IdentityStatement { get; set; } = string.Empty;

    [MaxLength(800)]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.Active;

    [MaxLength(24)]
    public string Color { get; set; } = "#22c55e";

    [MaxLength(80)]
    public string Icon { get; set; } = "user-round-check";

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];
}
