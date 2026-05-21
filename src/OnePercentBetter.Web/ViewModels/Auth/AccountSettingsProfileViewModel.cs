using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Auth;

public class AccountSettingsProfileViewModel
{
    [Required(ErrorMessage = "Informe seu nome.")]
    [MaxLength(120)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public DateTime? EmailConfirmedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
