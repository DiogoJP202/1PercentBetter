using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Habits;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class HabitsController : Controller
{
    private readonly HabitService _habitService;

    public HabitsController(HabitService habitService)
    {
        _habitService = habitService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _habitService.GetListAsync(User.GetRequiredUserId()));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await _habitService.CreateFormAsync(User.GetRequiredUserId()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HabitFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        if (!ModelState.IsValid)
        {
            var form = await _habitService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            viewModel.Identities = form.Identities;
            viewModel.Goals = form.Goals;
            return View(viewModel);
        }

        await _habitService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Habito criado.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var viewModel = await _habitService.EditFormAsync(User.GetRequiredUserId(), id);
        return viewModel is null ? NotFound() : View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HabitFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        viewModel.Id = id;

        if (!ModelState.IsValid)
        {
            var form = await _habitService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            viewModel.Identities = form.Identities;
            viewModel.Goals = form.Goals;
            return View(viewModel);
        }

        var updated = await _habitService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Habito atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        await _habitService.RegisterLogAsync(User.GetRequiredUserId(), id, HabitLogStatus.Completed);
        TempData["Success"] = "Habito concluido.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Fail(int id)
    {
        await _habitService.RegisterLogAsync(User.GetRequiredUserId(), id, HabitLogStatus.Failed);
        TempData["Warning"] = "Falha registrada. A versao de 2 minutos ainda pode salvar o dia.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Skip(int id)
    {
        await _habitService.RegisterLogAsync(User.GetRequiredUserId(), id, HabitLogStatus.Skipped);
        TempData["Info"] = "Habito pulado hoje.";
        return RedirectToAction(nameof(Index));
    }
}
