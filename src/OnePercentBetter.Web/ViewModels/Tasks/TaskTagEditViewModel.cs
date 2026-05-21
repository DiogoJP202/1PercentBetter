using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Tasks;

public class TaskTagEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da tag.")]
    [MaxLength(64)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(24)]
    [RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Escolha uma cor válida.")]
    public string Color { get; set; } = TaskVisualOptions.DefaultColor;
}
