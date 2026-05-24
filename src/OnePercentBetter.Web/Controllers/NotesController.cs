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

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] NoteFiltersViewModel filters)
    {
        return View(await _noteService.GetListAsync(User.GetRequiredUserId(), filters));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var note = await _noteService.GetDetailsAsync(User.GetRequiredUserId(), id);
        return note is null ? NotFound() : View(note);
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
            await PopulateFormOptionsAsync(userId, viewModel);
            return View(viewModel);
        }

        foreach (var error in await _noteService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            await PopulateFormOptionsAsync(userId, viewModel);
            return View(viewModel);
        }

        var noteId = await _noteService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Anotação criada com sucesso.";

        return RedirectToAction(nameof(Details), new { id = noteId });
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
            await PopulateFormOptionsAsync(userId, viewModel);
            return View(viewModel);
        }

        foreach (var error in await _noteService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            await PopulateFormOptionsAsync(userId, viewModel);
            return View(viewModel);
        }

        var updated = await _noteService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Anotação atualizada com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _noteService.DeleteAsync(User.GetRequiredUserId(), id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Anotação removida.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateFormOptionsAsync(string userId, NoteFormViewModel viewModel)
    {
        var form = await _noteService.CreateFormAsync(userId);
        viewModel.Identities = form.Identities;
        viewModel.Goals = form.Goals;
        viewModel.Habits = form.Habits;
        viewModel.TaskItems = form.TaskItems;
    }
}
