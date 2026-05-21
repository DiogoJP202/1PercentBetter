namespace OnePercentBetter.Web.Options;

public class ResendSettings
{
    public const string SectionName = "Resend";

    public string ApiToken { get; set; } = string.Empty;

    public string FromEmail { get; set; } = "onboarding@resend.dev";

    public string FromName { get; set; } = "1% Better";
}
