using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Services;
using OnePercentBetter.Web.ViewModels.Tasks;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly TaskItemService _taskItemService;

    public TasksController(TaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] TaskFiltersViewModel filters)
    {
        return View(await _taskItemService.GetListAsync(User.GetRequiredUserId(), filters));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var details = await _taskItemService.GetDetailsAsync(User.GetRequiredUserId(), id);
        return details is null ? NotFound() : View(details);
    }

    [HttpGet]
    public async Task<IActionResult> Create([FromQuery] DateTime? taskDate)
    {
        var viewModel = await _taskItemService.CreateFormAsync(User.GetRequiredUserId());
        if (taskDate.HasValue)
        {
            viewModel.TaskDate = taskDate.Value.Date;
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItemFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();

        if (!ModelState.IsValid)
        {
            await _taskItemService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        foreach (var error in await _taskItemService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            await _taskItemService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        await _taskItemService.CreateAsync(userId, viewModel);
        TempData["Success"] = "Tarefa criada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var form = await _taskItemService.EditFormAsync(User.GetRequiredUserId(), id);
        return form is null ? NotFound() : View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItemFormViewModel viewModel)
    {
        var userId = User.GetRequiredUserId();
        viewModel.Id = id;

        if (!ModelState.IsValid)
        {
            await _taskItemService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        foreach (var error in await _taskItemService.ValidateFormAsync(userId, viewModel))
        {
            ModelState.AddModelError(error.Key, error.Value);
        }

        if (!ModelState.IsValid)
        {
            await _taskItemService.PopulateOptionsAsync(viewModel, userId);
            return View(viewModel);
        }

        var updated = await _taskItemService.UpdateAsync(userId, viewModel);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Tarefa atualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskItemService.DeleteAsync(User.GetRequiredUserId(), id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Tarefa removida.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var changed = await _taskItemService.ChangeStatusAsync(User.GetRequiredUserId(), id, TaskItemStatus.Completed);
        if (!changed)
        {
            return NotFound();
        }

        TempData["Success"] = "Tarefa concluída.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reopen(int id)
    {
        var changed = await _taskItemService.ChangeStatusAsync(User.GetRequiredUserId(), id, TaskItemStatus.Pending);
        if (!changed)
        {
            return NotFound();
        }

        TempData["Info"] = "Tarefa reaberta.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var changed = await _taskItemService.ChangeStatusAsync(User.GetRequiredUserId(), id, TaskItemStatus.Cancelled);
        if (!changed)
        {
            return NotFound();
        }

        TempData["Warning"] = "Tarefa cancelada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Postpone(int id, int days = 1)
    {
        var changed = await _taskItemService.PostponeAsync(User.GetRequiredUserId(), id, days);
        if (!changed)
        {
            return NotFound();
        }

        TempData["Info"] = "Tarefa adiada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetTodayTasks()
    {
        return Json(await _taskItemService.GetTodayTasksAsync(User.GetRequiredUserId()));
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksByGoal(int goalId)
    {
        return Json(await _taskItemService.GetTasksByGoalAsync(User.GetRequiredUserId(), goalId));
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksByIdentity(int identityId)
    {
        return Json(await _taskItemService.GetTasksByIdentityAsync(User.GetRequiredUserId(), identityId));
    }

    [HttpGet]
    public async Task<IActionResult> GetCalendarTasks([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var userId = User.GetRequiredUserId();
        var startDate = (start ?? DateTime.Today.AddMonths(-1)).Date;
        var endDate = (end ?? DateTime.Today.AddMonths(1)).Date;

        var tasks = await _taskItemService.QueryUserTasks(userId)
            .Where(taskItem => taskItem.ShowOnCalendar
                && taskItem.TaskDate.HasValue
                && taskItem.TaskDate.Value >= startDate
                && taskItem.TaskDate.Value < endDate)
            .OrderBy(taskItem => taskItem.TaskDate)
            .ThenBy(taskItem => taskItem.StartTime)
            .Select(taskItem => new
            {
                id = taskItem.Id,
                title = taskItem.Title,
                status = taskItem.Status,
                priority = taskItem.Priority,
                taskDate = taskItem.TaskDate,
                startTime = taskItem.StartTime,
                endTime = taskItem.EndTime,
                dueDate = taskItem.DueDate,
                color = taskItem.Color,
                icon = taskItem.Icon
            })
            .ToListAsync();

        return Json(tasks);
    }
}
