using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Notes;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class NoteService
{
    private static readonly CultureInfo PtBrCulture = CultureInfo.GetCultureInfo("pt-BR");

    private readonly ApplicationDbContext _dbContext;
    private readonly IdentityService _identityService;
    private readonly GoalService _goalService;
    private readonly HabitService _habitService;
    private readonly TaskItemService _taskItemService;

    public NoteService(
        ApplicationDbContext dbContext,
        IdentityService identityService,
        GoalService goalService,
        HabitService habitService,
        TaskItemService taskItemService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _goalService = goalService;
        _habitService = habitService;
        _taskItemService = taskItemService;
    }

    public async Task<NoteListViewModel> GetListAsync(string userId, NoteFiltersViewModel? filters)
    {
        var normalizedFilters = await FillFilterOptionsAsync(filters ?? new NoteFiltersViewModel(), userId);
        var today = DateTime.Today;
        var monthStart = ParseMonthOrCurrent(normalizedFilters.Month, today);
        var monthEnd = monthStart.AddMonths(1);

        var query = _dbContext.Notes
            .AsNoTracking()
            .Where(note => note.UserId == userId);

        query = ApplyFilters(query, normalizedFilters, today, monthStart, monthEnd);

        var items = await query
            .OrderByDescending(note => note.Date)
            .ThenByDescending(note => note.CreatedAt)
            .Select(note => new NoteListItemViewModel
            {
                Id = note.Id,
                Title = note.Title,
                ContentPreview = note.Content.Length > 180 ? note.Content.Substring(0, 180) + "..." : note.Content,
                NoteType = note.NoteType,
                Tags = note.Tags,
                Date = note.Date,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                GoalTitle = note.Goal != null ? note.Goal.Title : null,
                IdentityName = note.Identity != null ? note.Identity.Name : null,
                HabitTitle = note.Habit != null ? note.Habit.Title : null,
                TaskTitle = note.TaskItem != null ? note.TaskItem.Title : null
            })
            .ToListAsync();

        var summaryBase = _dbContext.Notes
            .AsNoTracking()
            .Where(note => note.UserId == userId);

        var monthItems = await summaryBase
            .Where(note => note.Date >= monthStart && note.Date < monthEnd)
            .OrderByDescending(note => note.Date)
            .ThenByDescending(note => note.CreatedAt)
            .Take(6)
            .Select(note => new NoteListItemViewModel
            {
                Id = note.Id,
                Title = note.Title,
                ContentPreview = note.Content.Length > 180 ? note.Content.Substring(0, 180) + "..." : note.Content,
                NoteType = note.NoteType,
                Tags = note.Tags,
                Date = note.Date,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                GoalTitle = note.Goal != null ? note.Goal.Title : null,
                IdentityName = note.Identity != null ? note.Identity.Name : null,
                HabitTitle = note.Habit != null ? note.Habit.Title : null,
                TaskTitle = note.TaskItem != null ? note.TaskItem.Title : null
            })
            .ToListAsync();

        return new NoteListViewModel
        {
            Filters = normalizedFilters,
            Items = items,
            MonthItems = monthItems,
            TotalCount = await summaryBase.CountAsync(),
            MonthCount = await summaryBase.CountAsync(note => note.Date >= monthStart && note.Date < monthEnd),
            NotesWithTagsCount = await summaryBase.CountAsync(note => note.Tags != null && note.Tags != string.Empty),
            ReviewsCount = await summaryBase.CountAsync(note => note.NoteType == NoteType.WeeklyReview || note.NoteType == NoteType.MonthlyReview),
            MonthLabel = ToMonthLabel(monthStart)
        };
    }

    public async Task<NoteDetailsViewModel?> GetDetailsAsync(string userId, int id)
    {
        return await _dbContext.Notes
            .AsNoTracking()
            .Where(note => note.UserId == userId && note.Id == id)
            .Select(note => new NoteDetailsViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                NoteType = note.NoteType,
                Tags = note.Tags,
                Date = note.Date,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                GoalTitle = note.Goal != null ? note.Goal.Title : null,
                IdentityName = note.Identity != null ? note.Identity.Name : null,
                HabitTitle = note.Habit != null ? note.Habit.Title : null,
                TaskTitle = note.TaskItem != null ? note.TaskItem.Title : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<NoteFormViewModel> CreateFormAsync(string userId)
    {
        return await FillOptionsAsync(new NoteFormViewModel(), userId);
    }

    public async Task<NoteFormViewModel?> EditFormAsync(string userId, int id)
    {
        var note = await _dbContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (note is null)
        {
            return null;
        }

        return await FillOptionsAsync(new NoteFormViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            NoteType = note.NoteType,
            Tags = note.Tags,
            GoalId = note.GoalId,
            IdentityId = note.IdentityId,
            HabitId = note.HabitId,
            TaskItemId = note.TaskItemId,
            Date = note.Date,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        }, userId);
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, NoteFormViewModel viewModel)
    {
        var errors = new Dictionary<string, string>();

        if (viewModel.IdentityId.HasValue && !await _identityService.ExistsForUserAsync(userId, viewModel.IdentityId.Value))
        {
            errors[nameof(viewModel.IdentityId)] = "Identidade inválida para este usuário.";
        }

        if (viewModel.GoalId.HasValue && !await _goalService.ExistsForUserAsync(userId, viewModel.GoalId.Value))
        {
            errors[nameof(viewModel.GoalId)] = "Objetivo inválido para este usuário.";
        }

        if (viewModel.HabitId.HasValue && !await _habitService.ExistsForUserAsync(userId, viewModel.HabitId.Value))
        {
            errors[nameof(viewModel.HabitId)] = "Hábito inválido para este usuário.";
        }

        if (viewModel.TaskItemId.HasValue)
        {
            var taskExists = await _dbContext.TaskItems
                .AsNoTracking()
                .AnyAsync(taskItem => taskItem.UserId == userId && taskItem.Id == viewModel.TaskItemId.Value);

            if (!taskExists)
            {
                errors[nameof(viewModel.TaskItemId)] = "Tarefa inválida para este usuário.";
            }
        }

        return errors;
    }

    public async Task<int> CreateAsync(string userId, NoteFormViewModel viewModel)
    {
        var utcNow = DateTime.UtcNow;
        var note = new Note
        {
            UserId = userId,
            Title = viewModel.Title.Trim(),
            Content = viewModel.Content.Trim(),
            NoteType = viewModel.NoteType,
            Tags = viewModel.Tags?.Trim(),
            GoalId = viewModel.GoalId,
            IdentityId = viewModel.IdentityId,
            HabitId = viewModel.HabitId,
            TaskItemId = viewModel.TaskItemId,
            Date = viewModel.Date.Date,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        _dbContext.Notes.Add(note);
        await _dbContext.SaveChangesAsync();

        return note.Id;
    }

    public async Task<bool> UpdateAsync(string userId, NoteFormViewModel viewModel)
    {
        if (viewModel.Id is null)
        {
            return false;
        }

        var note = await _dbContext.Notes
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == viewModel.Id.Value);

        if (note is null)
        {
            return false;
        }

        note.Title = viewModel.Title.Trim();
        note.Content = viewModel.Content.Trim();
        note.NoteType = viewModel.NoteType;
        note.Tags = viewModel.Tags?.Trim();
        note.GoalId = viewModel.GoalId;
        note.IdentityId = viewModel.IdentityId;
        note.HabitId = viewModel.HabitId;
        note.TaskItemId = viewModel.TaskItemId;
        note.Date = viewModel.Date.Date;
        note.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string userId, int id)
    {
        var note = await _dbContext.Notes
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (note is null)
        {
            return false;
        }

        note.IsDeleted = true;
        note.DeletedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<NoteFiltersViewModel> FillFilterOptionsAsync(NoteFiltersViewModel filters, string userId)
    {
        var normalizedMonth = ParseMonthOrCurrent(filters.Month, DateTime.Today);

        filters.View = NormalizeView(filters.View);
        filters.Month = normalizedMonth.ToString("yyyy-MM", CultureInfo.InvariantCulture);
        filters.Tags = await GetTagOptionsAsync(userId);
        filters.Identities = await _identityService.GetOptionsAsync(userId);
        filters.Goals = await _goalService.GetOptionsAsync(userId);
        filters.Habits = await _habitService.GetOptionsAsync(userId);
        filters.TaskItems = await _taskItemService.GetOptionsAsync(userId);

        return filters;
    }

    private async Task<IReadOnlyList<SelectOptionViewModel>> GetTagOptionsAsync(string userId)
    {
        var rawTags = await _dbContext.Notes
            .AsNoTracking()
            .Where(note => note.UserId == userId && note.Tags != null && note.Tags != string.Empty)
            .Select(note => note.Tags!)
            .ToListAsync();

        return rawTags
            .SelectMany(SplitTags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag)
            .Select(tag => new SelectOptionViewModel
            {
                Value = tag,
                Text = tag
            })
            .ToList();
    }

    private static IQueryable<Note> ApplyFilters(
        IQueryable<Note> query,
        NoteFiltersViewModel filters,
        DateTime today,
        DateTime monthStart,
        DateTime monthEnd)
    {
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim();
            var pattern = $"%{search}%";

            query = query.Where(note =>
                EF.Functions.Like(note.Title, pattern)
                || EF.Functions.Like(note.Content, pattern)
                || (note.Tags != null && EF.Functions.Like(note.Tags, pattern)));
        }

        if (filters.NoteType.HasValue)
        {
            query = query.Where(note => note.NoteType == filters.NoteType.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Tag))
        {
            var tag = filters.Tag.Trim();
            var pattern = $"%{tag}%";
            query = query.Where(note => note.Tags != null && EF.Functions.Like(note.Tags, pattern));
        }

        if (filters.GoalId.HasValue)
        {
            query = query.Where(note => note.GoalId == filters.GoalId.Value);
        }

        if (filters.IdentityId.HasValue)
        {
            query = query.Where(note => note.IdentityId == filters.IdentityId.Value);
        }

        if (filters.HabitId.HasValue)
        {
            query = query.Where(note => note.HabitId == filters.HabitId.Value);
        }

        if (filters.TaskItemId.HasValue)
        {
            query = query.Where(note => note.TaskItemId == filters.TaskItemId.Value);
        }

        return filters.View switch
        {
            "today" => query.Where(note => note.Date == today),
            "week" => ApplyWeekFilter(query, today),
            "month" => query.Where(note => note.Date >= monthStart && note.Date < monthEnd),
            "reviews" => query.Where(note => note.NoteType == NoteType.WeeklyReview || note.NoteType == NoteType.MonthlyReview),
            _ => query
        };
    }

    private static IQueryable<Note> ApplyWeekFilter(IQueryable<Note> query, DateTime today)
    {
        var mondayOffset = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-mondayOffset);
        var weekEnd = weekStart.AddDays(7);

        return query.Where(note => note.Date >= weekStart && note.Date < weekEnd);
    }

    private static DateTime ParseMonthOrCurrent(string? monthValue, DateTime fallbackDate)
    {
        if (!string.IsNullOrWhiteSpace(monthValue)
            && DateTime.TryParseExact(monthValue, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedMonth))
        {
            return new DateTime(parsedMonth.Year, parsedMonth.Month, 1);
        }

        return new DateTime(fallbackDate.Year, fallbackDate.Month, 1);
    }

    private static string NormalizeView(string? view)
    {
        return (view ?? "all").Trim().ToLowerInvariant() switch
        {
            "today" => "today",
            "week" => "week",
            "month" => "month",
            "reviews" => "reviews",
            _ => "all"
        };
    }

    private static IReadOnlyList<string> SplitTags(string rawTags)
    {
        return rawTags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .ToList();
    }

    private static string ToMonthLabel(DateTime monthStart)
    {
        var monthName = PtBrCulture.TextInfo.ToTitleCase(monthStart.ToString("MMMM", PtBrCulture));
        return $"{monthName} {monthStart:yyyy}";
    }

    private async Task<NoteFormViewModel> FillOptionsAsync(NoteFormViewModel viewModel, string userId)
    {
        viewModel.Identities = await _identityService.GetOptionsAsync(userId);
        viewModel.Goals = await _goalService.GetOptionsAsync(userId);
        viewModel.Habits = await _habitService.GetOptionsAsync(userId);
        viewModel.TaskItems = await _taskItemService.GetOptionsAsync(userId);
        return viewModel;
    }
}
