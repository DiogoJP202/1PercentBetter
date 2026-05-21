namespace OnePercentBetter.Web.ViewModels.Auth;

public class AccountActionResultViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }

    public string? PrimaryActionLabel { get; set; }

    public string? PrimaryActionUrl { get; set; }

    public string? SecondaryActionLabel { get; set; }

    public string? SecondaryActionUrl { get; set; }
}
