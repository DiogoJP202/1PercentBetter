using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using OnePercentBetter.Web.Models.Identity;
using OnePercentBetter.Web.Options;
using Resend;

namespace OnePercentBetter.Web.Services;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ResendSettings _settings;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IResend resend, IOptions<ResendSettings> options, ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _settings = options.Value;
        _logger = logger;
    }

    public Task<bool> SendEmailConfirmationAsync(ApplicationUser user, string confirmationLink, CancellationToken cancellationToken = default)
    {
        var subject = "Ative sua conta no 1% Better";
        var headline = "Confirme seu e-mail para ativar sua conta";
        var intro = "Sua conta foi criada. Para liberar o login, confirme seu e-mail.";
        var buttonLabel = "Ativar minha conta";
        var footer = "Se você não criou essa conta, ignore este e-mail.";

        return SendBrandedEmailAsync(user, subject, headline, intro, buttonLabel, confirmationLink, footer, cancellationToken);
    }

    public Task<bool> SendPasswordResetAsync(ApplicationUser user, string resetLink, CancellationToken cancellationToken = default)
    {
        var subject = "Recupere sua senha do 1% Better";
        var headline = "Recebemos um pedido para redefinir sua senha";
        var intro = "Use o link abaixo para criar uma nova senha.";
        var buttonLabel = "Redefinir minha senha";
        var footer = "Se você não solicitou, ignore este e-mail. Sua senha atual continuará válida.";

        return SendBrandedEmailAsync(user, subject, headline, intro, buttonLabel, resetLink, footer, cancellationToken);
    }

    public Task<bool> SendAccountSecurityAlertAsync(ApplicationUser user, string message, CancellationToken cancellationToken = default)
    {
        var subject = "Aviso de segurança da sua conta 1% Better";
        var html = BuildTemplate(
            user.DisplayName,
            "Aviso de segurança",
            HtmlEncoder.Default.Encode(message),
            null,
            null,
            "Se você não reconhece esta atividade, altere sua senha.");

        return SendAsync(user.Email, subject, html, cancellationToken);
    }

    private Task<bool> SendBrandedEmailAsync(
        ApplicationUser user,
        string subject,
        string headline,
        string intro,
        string buttonLabel,
        string link,
        string footer,
        CancellationToken cancellationToken)
    {
        var html = BuildTemplate(
            user.DisplayName,
            headline,
            intro,
            buttonLabel,
            link,
            footer);

        return SendAsync(user.Email, subject, html, cancellationToken);
    }

    private async Task<bool> SendAsync(string? toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            _logger.LogWarning("Não foi possível enviar e-mail porque o destinatário está vazio. Subject={Subject}", subject);
            return false;
        }

        if (string.IsNullOrWhiteSpace(_settings.ApiToken)
            || string.IsNullOrWhiteSpace(_settings.FromEmail))
        {
            _logger.LogWarning(
                "Resend não configurado. Defina {Section} no appsettings/variáveis de ambiente para envio de e-mails. Subject={Subject}, To={To}",
                ResendSettings.SectionName,
                subject,
                toEmail);
            return false;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = new EmailMessage
            {
                From = FormatFromAddress(),
                Subject = subject,
                HtmlBody = htmlBody
            };

            message.To.Add(toEmail);
            await _resend.EmailSendAsync(message);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Falha ao enviar e-mail pelo Resend. Subject={Subject}, To={To}",
                subject,
                toEmail);
            return false;
        }
    }

    private string FormatFromAddress()
    {
        var fromEmail = _settings.FromEmail.Trim();
        var fromName = _settings.FromName.Trim();

        return string.IsNullOrWhiteSpace(fromName)
            ? fromEmail
            : $"{fromName} <{fromEmail}>";
    }

    private static string BuildTemplate(
        string? displayName,
        string headline,
        string intro,
        string? buttonLabel,
        string? buttonLink,
        string footer)
    {
        var safeName = string.IsNullOrWhiteSpace(displayName)
            ? "usuário"
            : HtmlEncoder.Default.Encode(displayName.Trim());

        var safeHeadline = HtmlEncoder.Default.Encode(headline);
        var safeIntro = HtmlEncoder.Default.Encode(intro);
        var safeFooter = HtmlEncoder.Default.Encode(footer);

        var buttonMarkup = string.Empty;
        var linkMarkup = string.Empty;

        if (!string.IsNullOrWhiteSpace(buttonLabel) && !string.IsNullOrWhiteSpace(buttonLink))
        {
            var safeButtonLabel = HtmlEncoder.Default.Encode(buttonLabel);
            var safeLink = HtmlEncoder.Default.Encode(buttonLink);

            buttonMarkup =
                $"<p style=\"margin:28px 0 16px;\">" +
                $"<a href=\"{safeLink}\" style=\"display:inline-block;padding:12px 20px;border-radius:12px;background:#34d399;color:#04111f;text-decoration:none;font-weight:700;\">{safeButtonLabel}</a>" +
                "</p>";

            linkMarkup =
                $"<p style=\"margin:0;color:#cbd5e1;font-size:12px;line-height:1.5;\">" +
                $"Se o botão não funcionar, copie e cole este link no navegador:<br/>" +
                $"<a href=\"{safeLink}\" style=\"color:#93c5fd;word-break:break-all;\">{safeLink}</a>" +
                "</p>";
        }

        return $$"""
<!doctype html>
<html lang="pt-BR">
<body style="margin:0;padding:24px;background:#020617;color:#e2e8f0;font-family:Segoe UI,Arial,sans-serif;">
  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:620px;margin:0 auto;border:1px solid rgba(148,163,184,0.25);border-radius:18px;background:#0f172a;">
    <tr>
      <td style="padding:28px 28px 14px;">
        <div style="font-size:12px;letter-spacing:.08em;text-transform:uppercase;color:#34d399;font-weight:700;">1% Better</div>
        <h1 style="margin:12px 0 0;font-size:24px;line-height:1.25;color:#ffffff;">{{safeHeadline}}</h1>
      </td>
    </tr>
    <tr>
      <td style="padding:0 28px 28px;">
        <p style="margin:0 0 12px;color:#e2e8f0;line-height:1.65;">Olá, {{safeName}}.</p>
        <p style="margin:0;color:#cbd5e1;line-height:1.65;">{{safeIntro}}</p>
        {{buttonMarkup}}
        {{linkMarkup}}
        <p style="margin:22px 0 0;color:#94a3b8;font-size:12px;line-height:1.6;">{{safeFooter}}</p>
      </td>
    </tr>
  </table>
</body>
</html>
""";
    }
}
