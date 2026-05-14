using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.CheckIns;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class CheckInsController : Controller
{
    private readonly CheckInService _checkInService;

    public CheckInsController(CheckInService checkInService)
    {
        _checkInService = checkInService;
    }

    [HttpGet]
    public async Task<IActionResult> Today()
    {
        return View(await _checkInService.GetTodayAsync(User.GetRequiredUserId()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Today(DailyCheckInViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        await _checkInService.CreateOrUpdateAsync(User.GetRequiredUserId(), viewModel);
        TempData["Success"] = "Check-in do dia salvo.";

        return RedirectToAction("Index", "Dashboard");
    }
}
