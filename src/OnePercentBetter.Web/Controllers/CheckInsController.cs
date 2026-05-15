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
    public async Task<IActionResult> Index(string period = "month", int? year = null, int? month = null)
    {
        var userId = User.GetRequiredUserId();
        var viewModel = await _checkInService.GetOverviewAsync(userId, period, year, month);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ChartData(string period = "month", int? year = null, int? month = null)
    {
        var userId = User.GetRequiredUserId();
        var points = await _checkInService.GetChartPointsAsync(userId, period, year, month);
        return Json(points);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(DateTime date)
    {
        var detail = await _checkInService.GetDetailAsync(User.GetRequiredUserId(), date);
        return Json(detail);
    }

    [HttpGet]
    public IActionResult Today()
    {
        return RedirectToAction(nameof(Edit), new { date = DateTime.Today.ToString("yyyy-MM-dd") });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Today(DailyCheckInViewModel viewModel)
    {
        return await SaveCheckInAsync(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(DateTime? date)
    {
        var targetDate = (date ?? DateTime.Today).Date;
        var viewModel = await _checkInService.GetByDateAsync(User.GetRequiredUserId(), targetDate);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DailyCheckInViewModel viewModel)
    {
        return await SaveCheckInAsync(viewModel);
    }

    private async Task<IActionResult> SaveCheckInAsync(DailyCheckInViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Edit", viewModel);
        }

        await _checkInService.CreateOrUpdateAsync(User.GetRequiredUserId(), viewModel);
        TempData["Success"] = "Check-in salvo.";

        return RedirectToAction(nameof(Index), new
        {
            period = "month",
            year = viewModel.Date.Year,
            month = viewModel.Date.Month
        });
    }
}
