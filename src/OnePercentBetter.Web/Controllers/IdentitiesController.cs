using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Identities;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class IdentitiesController : Controller
{
    private readonly IdentityService _identityService;

    public IdentitiesController(IdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _identityService.GetListAsync(User.GetRequiredUserId()));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await _identityService.CreateFormAsync(User.GetRequiredUserId()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IdentityFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        if (!ModelState.IsValid)
        {
            viewModel.Categories = (await _identityService.CreateFormAsync(userId)).Categories;
            return View(viewModel);
        }

        await _identityService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Identidade criada.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var viewModel = await _identityService.EditFormAsync(User.GetRequiredUserId(), id);
        return viewModel is null ? NotFound() : View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IdentityFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        viewModel.Id = id;

        if (!ModelState.IsValid)
        {
            viewModel.Categories = (await _identityService.CreateFormAsync(userId)).Categories;
            return View(viewModel);
        }

        var updated = await _identityService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Identidade atualizada.";
        return RedirectToAction(nameof(Index));
    }
}
