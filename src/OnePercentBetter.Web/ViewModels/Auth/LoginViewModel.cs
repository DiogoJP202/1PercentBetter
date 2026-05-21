using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public bool ShowResendConfirmationHint { get; set; }

    public string? PendingConfirmationEmail { get; set; }
}
