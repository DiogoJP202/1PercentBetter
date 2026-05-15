using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Habits;

public class HabitLocationCreateViewModel
{
    [Required(ErrorMessage = "Informe o nome do local.")]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;
}
