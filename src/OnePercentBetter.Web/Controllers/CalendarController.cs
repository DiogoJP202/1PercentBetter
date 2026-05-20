using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.Services;

namespace OnePercentBetter.Web.Controllers;

[Authorize]
public class CalendarController : Controller
{
    private readonly CalendarService _calendarService;

    public CalendarController(CalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _calendarService.GetOverviewAsync(User.GetRequiredUserId()));
    }

    [HttpGet]
    public async Task<IActionResult> Events([FromQuery] DateTime? start, [FromQuery] DateTime? end, [FromQuery] string? types)
    {
        var events = await _calendarService.GetEventsAsync(User.GetRequiredUserId(), start, end, ParseTypes(types));
        return Json(events);
    }

    [HttpGet]
    public async Task<IActionResult> Day([FromQuery] DateTime? date)
    {
        if (!date.HasValue)
        {
            return BadRequest(new { error = "Informe a data para consultar o calendário." });
        }

        return Json(await _calendarService.GetDayDetailsAsync(User.GetRequiredUserId(), date.Value));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HabitStatus(int id, DateTime date, HabitLogStatus status)
    {
        var updated = await _calendarService.RegisterHabitStatusAsync(User.GetRequiredUserId(), id, date, status);
        if (!updated)
        {
            return BadRequest(new { error = "Não foi possível atualizar este hábito." });
        }

        return Json(new { message = status == HabitLogStatus.Completed ? "Hábito concluído." : "Registro atualizado." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskStatus(int id, TaskItemStatus status)
    {
        var updated = await _calendarService.RegisterTaskStatusAsync(User.GetRequiredUserId(), id, status);
        if (!updated)
        {
            return BadRequest(new { error = "Nao foi possivel atualizar esta tarefa." });
        }

        return Json(new { message = status == TaskItemStatus.Completed ? "Tarefa concluida." : "Tarefa atualizada." });
    }

    private static IReadOnlySet<string> ParseTypes(string? types)
    {
        if (string.IsNullOrWhiteSpace(types))
        {
            return new HashSet<string>();
        }

        return types
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(type => type.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
