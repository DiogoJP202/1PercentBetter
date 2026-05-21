using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using OnePercentBetter.Web.Models.Identity;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Auth;

namespace OnePercentBetter.Web.Controllers;

public class AccountController : Controller
{
    private static readonly TimeSpan EmailThrottleWindow = TimeSpan.FromSeconds(45);

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly OnboardingService _onboardingService;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        OnboardingService onboardingService,
        IEmailService emailService,
        IMemoryCache memoryCache,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _onboardingService = onboardingService;
        _emailService = emailService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null, string? email = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            Email = email?.Trim() ?? string.Empty
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var email = (viewModel.Email ?? string.Empty).Trim();
        var result = await _signInManager.PasswordSignInAsync(
            email,
            viewModel.Password,
            viewModel.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null
                && string.IsNullOrWhiteSpace(viewModel.ReturnUrl)
                && !await _onboardingService.IsCompletedAsync(user.Id))
            {
                return RedirectToAction("Start", "Onboarding");
            }

            return await RedirectToLocal(viewModel.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Conta temporariamente bloqueada por excesso de tentativas.");
            return View(viewModel);
        }

        if (result.IsNotAllowed)
        {
            viewModel.ShowResendConfirmationHint = true;
            viewModel.PendingConfirmationEmail = email;
            ModelState.AddModelError(string.Empty, "Sua conta ainda não foi ativada. Verifique seu e-mail.");
            return View(viewModel);
        }

        ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
        return View(viewModel);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var email = viewModel.Email.Trim();
        var user = new ApplicationUser
        {
            DisplayName = (viewModel.DisplayName ?? string.Empty).Trim(),
            UserName = email,
            Email = email,
            EmailConfirmed = false,
            EmailConfirmedAt = null
        };

        var result = await _userManager.CreateAsync(user, viewModel.Password);
        if (result.Succeeded)
        {
            var sent = await TrySendEmailConfirmationAsync(user);
            TempData[sent ? "Success" : "Warning"] = sent
                ? "Conta criada! Enviamos um link de ativação para seu e-mail."
                : "Conta criada! Não foi possível enviar o e-mail agora. Use o reenvio de ativação.";

            return RedirectToAction(nameof(RegisterConfirmation), new { email });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(viewModel);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult RegisterConfirmation(string? email = null)
    {
        return View(new ResendConfirmationEmailViewModel
        {
            Email = email?.Trim() ?? string.Empty
        });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return View("AccountActionResult", BuildInvalidConfirmationTokenResult());
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return View("AccountActionResult", BuildInvalidConfirmationTokenResult());
        }

        var decodedToken = TryDecodeToken(token);
        if (decodedToken is null)
        {
            return View("AccountActionResult", BuildInvalidConfirmationTokenResult());
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            return View("AccountActionResult", BuildInvalidConfirmationTokenResult());
        }

        user.EmailConfirmedAt ??= DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            _logger.LogWarning("E-mail confirmado, mas não foi possível atualizar metadados do usuário. UserId={UserId}", user.Id);
        }

        return View("AccountActionResult", new AccountActionResultViewModel
        {
            IsSuccess = true,
            Title = "Conta ativada com sucesso",
            Message = "Agora você pode entrar no 1% Better e continuar seu onboarding.",
            PrimaryActionLabel = "Entrar",
            PrimaryActionUrl = Url.Action(nameof(Login), "Account")
        });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResendConfirmationEmail(string? email = null)
    {
        return View(new ResendConfirmationEmailViewModel
        {
            Email = email?.Trim() ?? string.Empty
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var email = viewModel.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null && !user.EmailConfirmed)
        {
            var throttleKey = $"confirm:{user.Id}";
            if (!IsThrottled(throttleKey))
            {
                await TrySendEmailConfirmationAsync(user);
            }
        }

        TempData["Info"] = "Se existir uma conta pendente para este e-mail, enviaremos um novo link de ativação.";
        return RedirectToAction(nameof(Login), new { email });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var email = viewModel.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null && user.EmailConfirmed)
        {
            var throttleKey = $"reset:{user.Id}";
            if (!IsThrottled(throttleKey))
            {
                await TrySendPasswordResetAsync(user);
            }
        }

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(string? userId, string? token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return View("AccountActionResult", BuildInvalidResetTokenResult());
        }

        var decodedToken = TryDecodeToken(token);
        if (decodedToken is null)
        {
            return View("AccountActionResult", BuildInvalidResetTokenResult());
        }

        return View(new ResetPasswordViewModel
        {
            UserId = userId,
            // Keep the encoded token in the form to avoid special-char issues on POST.
            Token = token
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var user = await _userManager.FindByIdAsync(viewModel.UserId);
        if (user is null)
        {
            return View("AccountActionResult", BuildInvalidResetTokenResult());
        }

        var decodedToken = TryDecodeToken(viewModel.Token);
        if (decodedToken is null)
        {
            return View("AccountActionResult", BuildInvalidResetTokenResult());
        }

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, viewModel.Password);
        if (result.Succeeded)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Sua senha foi atualizada com sucesso. Faça login para continuar.";
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, MapIdentityError(error));
        }

        return View(viewModel);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        return View(await BuildSettingsViewModelAsync(user));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSettings(AccountSettingsProfileViewModel profile)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var email = (profile.Email ?? string.Empty).Trim();
        var emailChanged = !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase);

        if (emailChanged)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is not null && existing.Id != user.Id)
            {
                ModelState.AddModelError(nameof(profile.Email), "Este e-mail já está em uso.");
            }
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildSettingsViewModelAsync(user, profile, new AccountChangePasswordViewModel());
            return View("Settings", invalidModel);
        }

        user.DisplayName = (profile.DisplayName ?? string.Empty).Trim();
        user.UpdatedAt = DateTime.UtcNow;

        if (emailChanged)
        {
            user.Email = email;
            user.UserName = email;
            user.EmailConfirmed = false;
            user.EmailConfirmedAt = null;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var invalidModel = await BuildSettingsViewModelAsync(user, profile, new AccountChangePasswordViewModel());
            return View("Settings", invalidModel);
        }

        if (emailChanged)
        {
            var sent = await TrySendEmailConfirmationAsync(user);
            TempData[sent ? "Success" : "Warning"] = sent
                ? "Dados atualizados. Confirmação enviada para o novo e-mail."
                : "Dados atualizados, mas não foi possível enviar confirmação agora.";
        }
        else
        {
            TempData["Success"] = "Dados da conta atualizados.";
        }

        await _signInManager.RefreshSignInAsync(user);
        return RedirectToAction(nameof(Settings));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(AccountChangePasswordViewModel password)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            var model = await BuildSettingsViewModelAsync(user, null, password);
            return View("Settings", model);
        }

        var result = await _userManager.ChangePasswordAsync(user, password.CurrentPassword, password.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            var model = await BuildSettingsViewModelAsync(user, null, password);
            return View("Settings", model);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);
        TempData["Success"] = "Senha alterada com sucesso.";
        return RedirectToAction(nameof(Settings));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmationFromSettings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (user.EmailConfirmed)
        {
            TempData["Info"] = "Seu e-mail já está confirmado.";
            return RedirectToAction(nameof(Settings));
        }

        var throttleKey = $"confirm:{user.Id}";
        if (IsThrottled(throttleKey))
        {
            TempData["Info"] = "Aguarde alguns segundos antes de solicitar um novo envio.";
            return RedirectToAction(nameof(Settings));
        }

        var sent = await TrySendEmailConfirmationAsync(user);
        TempData[sent ? "Success" : "Warning"] = sent
            ? "Novo e-mail de ativação enviado."
            : "Não foi possível reenviar o e-mail de ativação agora.";
        return RedirectToAction(nameof(Settings));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task<bool> TrySendEmailConfirmationAsync(ApplicationUser user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return false;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = EncodeToken(token);
        var confirmationLink = Url.Action(
            nameof(ConfirmEmail),
            "Account",
            new { userId = user.Id, token = encodedToken },
            protocol: Request.Scheme);

        if (string.IsNullOrWhiteSpace(confirmationLink))
        {
            _logger.LogWarning("Não foi possível gerar URL de confirmação. UserId={UserId}", user.Id);
            return false;
        }

        return await _emailService.SendEmailConfirmationAsync(user, confirmationLink);
    }

    private async Task<bool> TrySendPasswordResetAsync(ApplicationUser user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return false;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = EncodeToken(token);
        var resetLink = Url.Action(
            nameof(ResetPassword),
            "Account",
            new { userId = user.Id, token = encodedToken },
            protocol: Request.Scheme);

        if (string.IsNullOrWhiteSpace(resetLink))
        {
            _logger.LogWarning("Não foi possível gerar URL de reset de senha. UserId={UserId}", user.Id);
            return false;
        }

        return await _emailService.SendPasswordResetAsync(user, resetLink);
    }

    private bool IsThrottled(string key)
    {
        if (_memoryCache.TryGetValue(key, out _))
        {
            return true;
        }

        _memoryCache.Set(key, true, EmailThrottleWindow);
        return false;
    }

    private async Task<AccountSettingsViewModel> BuildSettingsViewModelAsync(
        ApplicationUser user,
        AccountSettingsProfileViewModel? profileOverride = null,
        AccountChangePasswordViewModel? passwordOverride = null)
    {
        var profile = profileOverride ?? new AccountSettingsProfileViewModel();

        if (profileOverride is null)
        {
            profile.DisplayName = user.DisplayName;
            profile.Email = user.Email ?? string.Empty;
        }

        profile.EmailConfirmed = user.EmailConfirmed;
        profile.EmailConfirmedAt = user.EmailConfirmedAt;
        profile.CreatedAt = user.CreatedAt;
        profile.UpdatedAt = user.UpdatedAt;

        return await Task.FromResult(new AccountSettingsViewModel
        {
            Profile = profile,
            Password = passwordOverride ?? new AccountChangePasswordViewModel()
        });
    }

    private static string EncodeToken(string token)
    {
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    private static string? TryDecodeToken(string encodedToken)
    {
        try
        {
            var decodedBytes = WebEncoders.Base64UrlDecode(encodedToken);
            return Encoding.UTF8.GetString(decodedBytes);
        }
        catch
        {
            return null;
        }
    }

    private AccountActionResultViewModel BuildInvalidConfirmationTokenResult()
    {
        return new AccountActionResultViewModel
        {
            IsSuccess = false,
            Title = "Link inválido ou expirado",
            Message = "Esse link expirou ou já foi utilizado. Solicite um novo link para continuar.",
            PrimaryActionLabel = "Reenviar ativação",
            PrimaryActionUrl = Url.Action(nameof(ResendConfirmationEmail), "Account"),
            SecondaryActionLabel = "Entrar",
            SecondaryActionUrl = Url.Action(nameof(Login), "Account")
        };
    }

    private AccountActionResultViewModel BuildInvalidResetTokenResult()
    {
        return new AccountActionResultViewModel
        {
            IsSuccess = false,
            Title = "Link inválido ou expirado",
            Message = "Esse link de redefinição expirou ou já foi utilizado. Solicite um novo link para continuar.",
            PrimaryActionLabel = "Recuperar senha",
            PrimaryActionUrl = Url.Action(nameof(ForgotPassword), "Account"),
            SecondaryActionLabel = "Entrar",
            SecondaryActionUrl = Url.Action(nameof(Login), "Account")
        };
    }

    private async Task<IActionResult> RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is not null && !await _onboardingService.IsCompletedAsync(user.Id))
        {
            return RedirectToAction("Start", "Onboarding");
        }

        return RedirectToAction("Index", "Dashboard");
    }

    private static string MapIdentityError(IdentityError error)
    {
        return error.Code switch
        {
            "InvalidToken" => "Esse link de redefinição expirou ou é inválido. Solicite um novo link.",
            _ => error.Description
        };
    }
}
