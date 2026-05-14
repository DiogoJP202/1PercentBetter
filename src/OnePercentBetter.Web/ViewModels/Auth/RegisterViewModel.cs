using System.ComponentModel.DataAnnotations;

namespace OnePercentBetter.Web.ViewModels.Auth;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Informe seu nome.")]
    [MaxLength(120)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme sua senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "As senhas nao conferem.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
