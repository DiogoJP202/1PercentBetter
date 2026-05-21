namespace OnePercentBetter.Web.ViewModels.Auth;

public class AccountSettingsViewModel
{
    public AccountSettingsProfileViewModel Profile { get; set; } = new();

    public AccountChangePasswordViewModel Password { get; set; } = new();
}
