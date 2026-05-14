using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Onboarding;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class OnboardingController : Controller
{
    private readonly OnboardingService _onboardingService;

    public OnboardingController(OnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(await _onboardingService.CreateFormAsync(userId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(OnboardingViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        if (!ModelState.IsValid)
        {
            var form = await _onboardingService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            return View(viewModel);
        }

        await _onboardingService.CompleteAsync(userId, viewModel);
        TempData["Success"] = "Seu primeiro sistema de evolucao foi criado.";

        return RedirectToAction("Index", "Dashboard");
    }
}
