using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Auth;

public class ResendConfirmationEmailViewModel
{
    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;
}
