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
    private readonly HabitLocationService _habitLocationService;
    private readonly SimpleHabitService _simpleHabitService;

    public HabitsController(
        HabitService habitService,
        HabitLocationService habitLocationService,
        SimpleHabitService simpleHabitService)
    {
        _habitService = habitService;
        _habitLocationService = habitLocationService;
        _simpleHabitService = simpleHabitService;
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
            await _habitService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        foreach (var error in await _habitService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            await _habitService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        await _habitService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Hábito criado.";

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
            await _habitService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        foreach (var error in await _habitService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            await _habitService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        var updated = await _habitService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Hábito atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLocation(HabitLocationCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var message = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .FirstOrDefault() ?? "Não foi possível cadastrar o local.";

            return BadRequest(new { error = message });
        }

        var result = await _habitLocationService.CreateAsync(User.GetRequiredUserId(), viewModel);
        if (!result.Success || result.Option is null)
        {
            return BadRequest(new { error = result.Error ?? "Não foi possível cadastrar o local." });
        }

        return Json(new
        {
            value = result.Option.Value,
            text = result.Option.Text
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSimpleHabit(SimpleHabitCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var message = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .FirstOrDefault() ?? "Não foi possível cadastrar o hábito simples.";

            return BadRequest(new { error = message });
        }

        var userId = User.GetRequiredUserId();
        var result = viewModel.Id.HasValue
            ? await _simpleHabitService.UpdateAsync(userId, viewModel)
            : await _simpleHabitService.CreateAsync(userId, viewModel);

        if (!result.Success || result.Option is null)
        {
            return BadRequest(new { error = result.Error ?? "Não foi possível cadastrar o hábito simples." });
        }

        return Json(new
        {
            id = result.Option.Value,
            value = $"simple:{result.Option.Value}",
            text = result.Option.Text
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        await _habitService.RegisterLogAsync(User.GetRequiredUserId(), id, HabitLogStatus.Completed);
        TempData["Success"] = "Hábito concluído.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Fail(int id)
    {
        await _habitService.RegisterLogAsync(User.GetRequiredUserId(), id, HabitLogStatus.Failed);
        TempData["Warning"] = "Falha registrada. A versão de 2 minutos ainda pode salvar o dia.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Skip(int id)
    {
        await _habitService.RegisterLogAsync(User.GetRequiredUserId(), id, HabitLogStatus.Skipped);
        TempData["Info"] = "Hábito pulado hoje.";
        return RedirectToAction(nameof(Index));
    }
}

