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
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Start));
    }

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        if (await _onboardingService.NeedsTourAsync(userId))
        {
            return RedirectToAction(nameof(Tour));
        }

        return RedirectToAction(nameof(Setup));
    }

    [HttpGet]
    public async Task<IActionResult> Tour(bool replay = false)
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            if (!replay)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        if (!replay && await _onboardingService.HasSeenTourAsync(userId))
        {
            return RedirectToAction(nameof(Setup));
        }

        return View();
    }

    [HttpGet]
    public IActionResult ReviewTour()
    {
        return RedirectToAction(nameof(Tour), new { replay = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteTour()
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        await _onboardingService.MarkTourCompletedAsync(userId);
        return RedirectToAction(nameof(Setup));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SkipTour()
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        await _onboardingService.MarkTourSkippedAsync(userId);
        return RedirectToAction(nameof(Setup));
    }

    [HttpGet]
    public async Task<IActionResult> Setup()
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        if (await _onboardingService.NeedsTourAsync(userId))
        {
            return RedirectToAction(nameof(Tour));
        }

        return View("Index", await _onboardingService.CreateFormAsync(userId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup(OnboardingViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        if (await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        if (await _onboardingService.NeedsTourAsync(userId))
        {
            return RedirectToAction(nameof(Tour));
        }

        if (!ModelState.IsValid)
        {
            var form = await _onboardingService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            return View("Index", viewModel);
        }

        foreach (var error in await _onboardingService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            var form = await _onboardingService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            return View("Index", viewModel);
        }

        await _onboardingService.CompleteAsync(userId, viewModel);
        TempData["Success"] = "Seu primeiro sistema de evolucao foi criado.";

        return RedirectToAction("Index", "Dashboard");
    }
}
