using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Goals;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class GoalsController : Controller
{
    private readonly GoalService _goalService;

    public GoalsController(GoalService goalService)
    {
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _goalService.GetListAsync(User.GetRequiredUserId()));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await _goalService.CreateFormAsync(User.GetRequiredUserId()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GoalFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        if (!ModelState.IsValid)
        {
            var form = await _goalService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            viewModel.Identities = form.Identities;
            return View(viewModel);
        }

        foreach (var error in await _goalService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            var form = await _goalService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            viewModel.Identities = form.Identities;
            return View(viewModel);
        }

        await _goalService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Objetivo criado.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var viewModel = await _goalService.EditFormAsync(User.GetRequiredUserId(), id);
        return viewModel is null ? NotFound() : View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GoalFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        viewModel.Id = id;

        if (!ModelState.IsValid)
        {
            var form = await _goalService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            viewModel.Identities = form.Identities;
            return View(viewModel);
        }

        foreach (var error in await _goalService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            var form = await _goalService.CreateFormAsync(userId);
            viewModel.Categories = form.Categories;
            viewModel.Identities = form.Identities;
            return View(viewModel);
        }

        var updated = await _goalService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Objetivo atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        await _goalService.ChangeStatusAsync(User.GetRequiredUserId(), id, ItemStatus.Completed);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pause(int id)
    {
        await _goalService.ChangeStatusAsync(User.GetRequiredUserId(), id, ItemStatus.Paused);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _goalService.DeleteAsync(User.GetRequiredUserId(), id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Objetivo excluído. Hábitos e anotações vinculados foram preservados.";
        return RedirectToAction(nameof(Index));
    }
}
