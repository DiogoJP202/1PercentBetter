using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Onboarding;

public class OnboardingViewModel
{
    [Required(ErrorMessage = "Escolha uma área de foco.")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Nomeie a identidade.")]
    [MaxLength(120)]
    public string IdentityName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escreva a frase da identidade.")]
    [MaxLength(260)]
    public string IdentityStatement { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o primeiro objetivo.")]
    [MaxLength(160)]
    public string GoalTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o primeiro hábito.")]
    [MaxLength(160)]
    public string HabitTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Defina a versão de 2 minutos.")]
    [MaxLength(260)]
    public string TwoMinuteVersion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Defina o gatilho.")]
    [MaxLength(260)]
    public string Trigger { get; set; } = string.Empty;

    [MaxLength(260)]
    public string? Reward { get; set; }

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];
}
