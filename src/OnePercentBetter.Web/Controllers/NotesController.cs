using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Notes;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class NotesController : Controller
{
    private readonly NoteService _noteService;

    public NotesController(NoteService noteService)
    {
        _noteService = noteService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _noteService.GetListAsync(User.GetRequiredUserId()));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await _noteService.CreateFormAsync(User.GetRequiredUserId()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoteFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        if (!ModelState.IsValid)
        {
            var form = await _noteService.CreateFormAsync(userId);
            viewModel.Identities = form.Identities;
            viewModel.Goals = form.Goals;
            viewModel.Habits = form.Habits;
            return View(viewModel);
        }

        foreach (var error in await _noteService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            var form = await _noteService.CreateFormAsync(userId);
            viewModel.Identities = form.Identities;
            viewModel.Goals = form.Goals;
            viewModel.Habits = form.Habits;
            return View(viewModel);
        }

        await _noteService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Anotacao criada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var viewModel = await _noteService.EditFormAsync(User.GetRequiredUserId(), id);
        return viewModel is null ? NotFound() : View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NoteFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        viewModel.Id = id;

        if (!ModelState.IsValid)
        {
            var form = await _noteService.CreateFormAsync(userId);
            viewModel.Identities = form.Identities;
            viewModel.Goals = form.Goals;
            viewModel.Habits = form.Habits;
            return View(viewModel);
        }

        foreach (var error in await _noteService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            var form = await _noteService.CreateFormAsync(userId);
            viewModel.Identities = form.Identities;
            viewModel.Goals = form.Goals;
            viewModel.Habits = form.Habits;
            return View(viewModel);
        }

        var updated = await _noteService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Anotacao atualizada com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}
