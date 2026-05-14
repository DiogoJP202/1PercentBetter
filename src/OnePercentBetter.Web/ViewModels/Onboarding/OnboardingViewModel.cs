using System.ComponentModel.DataAnnotations;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.ViewModels.Onboarding;

public class OnboardingViewModel
{
    [Required(ErrorMessage = "Escolha uma area de foco.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Nomeie a identidade.")]
    [MaxLength(120)]
    public string IdentityName { get; set; } = "Desenvolvedor .NET consistente";

    [Required(ErrorMessage = "Escreva a frase da identidade.")]
    [MaxLength(260)]
    public string IdentityStatement { get; set; } = "Eu sou uma pessoa que evolui tecnicamente todos os dias.";

    [Required(ErrorMessage = "Informe o primeiro objetivo.")]
    [MaxLength(160)]
    public string GoalTitle { get; set; } = "Evoluir em ASP.NET Core MVC";

    [Required(ErrorMessage = "Informe o primeiro habito.")]
    [MaxLength(160)]
    public string HabitTitle { get; set; } = "Estudar ASP.NET Core por 20 minutos";

    [Required(ErrorMessage = "Defina a versao de 2 minutos.")]
    [MaxLength(260)]
    public string TwoMinuteVersion { get; set; } = "Abrir o projeto e revisar uma controller.";

    [Required(ErrorMessage = "Defina o gatilho.")]
    [MaxLength(260)]
    public string Trigger { get; set; } = "Depois do cafe da noite.";

    [MaxLength(260)]
    public string? Reward { get; set; } = "Marcar progresso no dashboard.";

    public IReadOnlyList<SelectOptionViewModel> Categories { get; set; } = [];
}
