using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Services;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;
    private readonly OnboardingService _onboardingService;

    public DashboardController(DashboardService dashboardService, OnboardingService onboardingService)
    {
        _dashboardService = dashboardService;
        _onboardingService = onboardingService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetRequiredUserId();
        if (!await _onboardingService.IsCompletedAsync(userId))
        {
            return RedirectToAction("Index", "Onboarding");
        }

        var viewModel = await _dashboardService.GetDashboardAsync(userId);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetWeeklyProgress()
    {
        var data = await _dashboardService.GetWeeklyProgressAsync(User.GetRequiredUserId());
        return Json(data);
    }
}
