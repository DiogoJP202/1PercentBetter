using OnePercentBetter.Web.Models.Identity;

namespace OnePercentBetter.Web.Services;

public interface IEmailService
{
    Task<bool> SendEmailConfirmationAsync(ApplicationUser user, string confirmationLink, CancellationToken cancellationToken = default);

    Task<bool> SendPasswordResetAsync(ApplicationUser user, string resetLink, CancellationToken cancellationToken = default);

    Task<bool> SendAccountSecurityAlertAsync(ApplicationUser user, string message, CancellationToken cancellationToken = default);
}
