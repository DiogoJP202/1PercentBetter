using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Habits;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class HabitService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryService _categoryService;
    private readonly IdentityService _identityService;
    private readonly GoalService _goalService;
    private readonly HabitLocationService _habitLocationService;
    private readonly SimpleHabitService _simpleHabitService;

    public HabitService(
        ApplicationDbContext dbContext,
        CategoryService categoryService,
        IdentityService identityService,
        GoalService goalService,
        HabitLocationService habitLocationService,
        SimpleHabitService simpleHabitService)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
        _identityService = identityService;
        _goalService = goalService;
        _habitLocationService = habitLocationService;
        _simpleHabitService = simpleHabitService;
    }

    public async Task<IReadOnlyList<HabitListItemViewModel>> GetListAsync(string userId)
    {
        var today = DateTime.Today;

        var habits = await _dbContext.Habits
            .AsNoTracking()
            .Where(habit => habit.UserId == userId)
            .OrderByDescending(habit => habit.CreatedAt)
            .Select(habit => new
            {
                habit.Id,
                habit.Title,
                habit.TwoMinuteVersion,
                habit.Trigger,
                IdentityName = habit.Identity != null ? habit.Identity.Name : null,
                GoalTitle = habit.Goal != null ? habit.Goal.Title : null,
                CategoryName = habit.Category != null ? habit.Category.Name : null,
                LocationName = habit.Location != null ? habit.Location.Name : null,
                StackedAfterHabitTitle = habit.StackedAfterHabit != null ? habit.StackedAfterHabit.Title : null,
                StackedAfterSimpleHabitName = habit.StackedAfterSimpleHabit != null ? habit.StackedAfterSimpleHabit.Name : null,
                StackedAfterSimpleHabitTime = habit.StackedAfterSimpleHabit != null ? habit.StackedAfterSimpleHabit.ScheduledTime : null,
                habit.Status,
                habit.FrequencyType,
                habit.SuggestedTime,
                TodayStatus = habit.Logs
                    .Where(log => log.Date == today)
                    .Select(log => (HabitLogStatus?)log.Status)
                    .FirstOrDefault(),
                habit.Color,
                habit.Icon
            })
            .ToListAsync();

        return habits
            .Select(habit => new HabitListItemViewModel
            {
                Id = habit.Id,
                Title = habit.Title,
                TwoMinuteVersion = habit.TwoMinuteVersion,
                Trigger = habit.Trigger,
                IdentityName = habit.IdentityName,
                GoalTitle = habit.GoalTitle,
                CategoryName = habit.CategoryName,
                LocationName = habit.LocationName,
                StackedAfterHabitTitle = habit.StackedAfterHabitTitle,
                StackedAfterText = habit.StackedAfterHabitTitle
                    ?? (habit.StackedAfterSimpleHabitName is not null
                        ? SimpleHabitService.BuildLabel(habit.StackedAfterSimpleHabitName, habit.StackedAfterSimpleHabitTime)
                        : null),
                Status = habit.Status,
                FrequencyType = habit.FrequencyType,
                SuggestedTime = habit.SuggestedTime,
                TodayStatus = habit.TodayStatus,
                Color = habit.Color,
                Icon = habit.Icon
            })
            .ToList();
    }

    public async Task<HabitFormViewModel> CreateFormAsync(string userId)
    {
        return await PopulateOptionsAsync(new HabitFormViewModel(), userId);
    }

    public async Task<HabitFormViewModel?> EditFormAsync(string userId, int id)
    {
        var habit = await _dbContext.Habits
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (habit is null)
        {
            return null;
        }

        return await PopulateOptionsAsync(new HabitFormViewModel
        {
            Id = habit.Id,
            Title = habit.Title,
            Description = habit.Description,
            IdentityId = habit.IdentityId,
            GoalId = habit.GoalId,
            CategoryId = habit.CategoryId,
            LocationId = habit.LocationId,
            StackedAfterHabitId = habit.StackedAfterHabitId,
            StackedAfterSimpleHabitId = habit.StackedAfterSimpleHabitId,
            StackBaseKey = BuildStackBaseKey(habit.StackedAfterHabitId, habit.StackedAfterSimpleHabitId),
            FrequencyType = habit.FrequencyType,
            DaysOfWeek = habit.DaysOfWeek,
            SelectedDaysOfWeek = ParseDaysOfWeek(habit.DaysOfWeek),
            SuggestedTime = habit.SuggestedTime,
            Difficulty = habit.Difficulty,
            TwoMinuteVersion = habit.TwoMinuteVersion,
            Trigger = habit.Trigger,
            Reward = habit.Reward,
            Status = habit.Status,
            Color = habit.Color,
            Icon = habit.Icon
        }, userId);
    }

    public async Task<HabitFormViewModel> PopulateOptionsAsync(HabitFormViewModel viewModel, string userId)
    {
        viewModel.Categories = await _categoryService.GetOptionsAsync(userId);
        viewModel.Identities = await _identityService.GetOptionsAsync(userId);
        viewModel.Goals = await _goalService.GetOptionsAsync(userId);
        viewModel.Locations = await _habitLocationService.GetOptionsAsync(userId);
        viewModel.StackableHabits = await GetStackableHabitOptionsAsync(userId, viewModel.Id);
        viewModel.SimpleHabits = await _simpleHabitService.GetOptionsAsync(userId);
        return viewModel;
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, HabitFormViewModel viewModel)
    {
        var errors = new Dictionary<string, string>();

        if (viewModel.FrequencyType == HabitFrequencyType.Custom)
        {
            errors[nameof(viewModel.FrequencyType)] = "Frequência personalizada ainda não está disponível.";
        }

        if (viewModel.FrequencyType == HabitFrequencyType.SpecificDays && viewModel.SelectedDaysOfWeek.Count == 0)
        {
            errors[nameof(viewModel.SelectedDaysOfWeek)] = "Escolha pelo menos um dia da semana.";
        }

        if (viewModel.CategoryId.HasValue && !await CategoryBelongsToUserAsync(userId, viewModel.CategoryId.Value))
        {
            errors[nameof(viewModel.CategoryId)] = "Categoria inválida para este usuário.";
        }

        if (viewModel.IdentityId.HasValue && !await IdentityBelongsToUserAsync(userId, viewModel.IdentityId.Value))
        {
            errors[nameof(viewModel.IdentityId)] = "Identidade inválida para este usuário.";
        }

        if (viewModel.GoalId.HasValue && !await GoalBelongsToUserAsync(userId, viewModel.GoalId.Value))
        {
            errors[nameof(viewModel.GoalId)] = "Objetivo inválido para este usuário.";
        }

        if (viewModel.LocationId.HasValue && !await _habitLocationService.ExistsForUserAsync(userId, viewModel.LocationId.Value))
        {
            errors[nameof(viewModel.LocationId)] = "Local inválido para este usuário.";
        }

        var stackError = await ApplyStackingSelectionAsync(userId, viewModel);
        if (stackError is not null)
        {
            errors[nameof(viewModel.StackBaseKey)] = stackError;
        }

        if (!HabitVisualOptions.IsAllowedIcon(viewModel.Icon))
        {
            errors[nameof(viewModel.Icon)] = "Escolha um ícone válido.";
        }

        return errors;
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId)
    {
        return await _dbContext.Habits
            .AsNoTracking()
            .Where(habit => habit.UserId == userId)
            .OrderBy(habit => habit.Title)
            .Select(habit => new SelectOptionViewModel
            {
                Value = habit.Id.ToString(),
                Text = habit.Title
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsForUserAsync(string userId, int habitId)
    {
        return await _dbContext.Habits
            .AsNoTracking()
            .AnyAsync(habit => habit.Id == habitId && habit.UserId == userId);
    }

    public async Task<int> CreateAsync(string userId, HabitFormViewModel viewModel)
    {
        var habit = new Habit
        {
            UserId = userId,
            Title = viewModel.Title.Trim(),
            Description = viewModel.Description?.Trim(),
            IdentityId = viewModel.IdentityId,
            GoalId = viewModel.GoalId,
            CategoryId = viewModel.CategoryId,
            LocationId = viewModel.LocationId,
            StackedAfterHabitId = viewModel.StackedAfterHabitId,
            StackedAfterSimpleHabitId = viewModel.StackedAfterSimpleHabitId,
            FrequencyType = viewModel.FrequencyType,
            DaysOfWeek = BuildDaysOfWeek(viewModel),
            SuggestedTime = viewModel.SuggestedTime,
            Difficulty = viewModel.Difficulty,
            TwoMinuteVersion = viewModel.TwoMinuteVersion.Trim(),
            Trigger = viewModel.Trigger.Trim(),
            Reward = viewModel.Reward?.Trim(),
            Status = viewModel.Status,
            Color = NormalizeColor(viewModel.Color),
            Icon = NormalizeIcon(viewModel.Icon)
        };

        _dbContext.Habits.Add(habit);
        await _dbContext.SaveChangesAsync();

        return habit.Id;
    }

    public async Task<bool> UpdateAsync(string userId, HabitFormViewModel viewModel)
    {
        if (viewModel.Id is null)
        {
            return false;
        }

        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == viewModel.Id.Value);

        if (habit is null)
        {
            return false;
        }

        habit.Title = viewModel.Title.Trim();
        habit.Description = viewModel.Description?.Trim();
        habit.IdentityId = viewModel.IdentityId;
        habit.GoalId = viewModel.GoalId;
        habit.CategoryId = viewModel.CategoryId;
        habit.LocationId = viewModel.LocationId;
        habit.StackedAfterHabitId = viewModel.StackedAfterHabitId;
        habit.StackedAfterSimpleHabitId = viewModel.StackedAfterSimpleHabitId;
        habit.FrequencyType = viewModel.FrequencyType;
        habit.DaysOfWeek = BuildDaysOfWeek(viewModel);
        habit.SuggestedTime = viewModel.SuggestedTime;
        habit.Difficulty = viewModel.Difficulty;
        habit.TwoMinuteVersion = viewModel.TwoMinuteVersion.Trim();
        habit.Trigger = viewModel.Trigger.Trim();
        habit.Reward = viewModel.Reward?.Trim();
        habit.Status = viewModel.Status;
        habit.Color = NormalizeColor(viewModel.Color);
        habit.Icon = NormalizeIcon(viewModel.Icon);
        habit.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RegisterLogAsync(string userId, int habitId, HabitLogStatus status)
    {
        var habitExists = await _dbContext.Habits
            .AnyAsync(habit => habit.UserId == userId && habit.Id == habitId);

        if (!habitExists)
        {
            return false;
        }

        var today = DateTime.Today;
        var log = await _dbContext.HabitLogs
            .FirstOrDefaultAsync(item => item.UserId == userId && item.HabitId == habitId && item.Date == today);

        if (log is null)
        {
            log = new HabitLog
            {
                UserId = userId,
                HabitId = habitId,
                Date = today
            };
            _dbContext.HabitLogs.Add(log);
        }

        log.Status = status;
        log.CompletedAt = status == HabitLogStatus.Completed ? DateTime.UtcNow : null;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<bool> CategoryBelongsToUserAsync(string userId, int categoryId)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .AnyAsync(category => category.Id == categoryId && (category.UserId == null || category.UserId == userId));
    }

    private async Task<bool> IdentityBelongsToUserAsync(string userId, int identityId)
    {
        return await _dbContext.UserIdentities
            .AsNoTracking()
            .AnyAsync(identity => identity.Id == identityId && identity.UserId == userId);
    }

    private async Task<bool> GoalBelongsToUserAsync(string userId, int goalId)
    {
        return await _dbContext.Goals
            .AsNoTracking()
            .AnyAsync(goal => goal.Id == goalId && goal.UserId == userId);
    }

    private async Task<IReadOnlyList<SelectOptionViewModel>> GetStackableHabitOptionsAsync(string userId, int? currentHabitId)
    {
        var habits = await _dbContext.Habits
            .AsNoTracking()
            .Where(habit => habit.UserId == userId && (!currentHabitId.HasValue || habit.Id != currentHabitId.Value))
            .OrderBy(habit => habit.Title)
            .Select(habit => new
            {
                habit.Id,
                habit.Title,
                habit.FrequencyType,
                habit.SuggestedTime
            })
            .ToListAsync();

        return habits
            .Select(habit => new SelectOptionViewModel
            {
                Value = habit.Id.ToString(),
                Text = BuildStackableHabitLabel(habit.Title, habit.FrequencyType, habit.SuggestedTime)
            })
            .ToList();
    }

    private async Task<bool> WouldCreateStackingCycleAsync(string userId, int habitId, int stackedAfterHabitId)
    {
        var currentId = stackedAfterHabitId;

        for (var depth = 0; depth < 25; depth++)
        {
            if (currentId == habitId)
            {
                return true;
            }

            var nextId = await _dbContext.Habits
                .AsNoTracking()
                .Where(habit => habit.UserId == userId && habit.Id == currentId)
                .Select(habit => habit.StackedAfterHabitId)
                .FirstOrDefaultAsync();

            if (!nextId.HasValue)
            {
                return false;
            }

            currentId = nextId.Value;
        }

        return true;
    }

    private async Task<string?> ApplyStackingSelectionAsync(string userId, HabitFormViewModel viewModel)
    {
        viewModel.StackedAfterHabitId = null;
        viewModel.StackedAfterSimpleHabitId = null;

        if (string.IsNullOrWhiteSpace(viewModel.StackBaseKey))
        {
            return null;
        }

        var segments = viewModel.StackBaseKey.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 2 || !int.TryParse(segments[1], out var id))
        {
            return "Escolha uma base de empilhamento válida.";
        }

        if (segments[0] == "habit")
        {
            if (viewModel.Id.HasValue && id == viewModel.Id.Value)
            {
                return "Um hábito não pode ser empilhado depois dele mesmo.";
            }

            if (!await ExistsForUserAsync(userId, id))
            {
                return "Hábito base inválido para este usuário.";
            }

            if (viewModel.Id.HasValue && await WouldCreateStackingCycleAsync(userId, viewModel.Id.Value, id))
            {
                return "Escolha outro hábito base para evitar um ciclo de empilhamento.";
            }

            viewModel.StackedAfterHabitId = id;
            return null;
        }

        if (segments[0] == "simple")
        {
            if (!await _simpleHabitService.ExistsForUserAsync(userId, id))
            {
                return "Hábito simples inválido para este usuário.";
            }

            viewModel.StackedAfterSimpleHabitId = id;
            return null;
        }

        return "Escolha uma base de empilhamento válida.";
    }

    private static string? BuildDaysOfWeek(HabitFormViewModel viewModel)
    {
        if (viewModel.FrequencyType != HabitFrequencyType.SpecificDays)
        {
            return null;
        }

        var selectedDays = viewModel.SelectedDaysOfWeek
            .Distinct()
            .OrderBy(day => (int)day)
            .Select(day => day.ToString())
            .ToList();

        return selectedDays.Count == 0 ? null : string.Join(',', selectedDays);
    }

    private static List<DayOfWeek> ParseDaysOfWeek(string? daysOfWeek)
    {
        if (string.IsNullOrWhiteSpace(daysOfWeek))
        {
            return [];
        }

        return daysOfWeek
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(day => Enum.TryParse<DayOfWeek>(day, true, out var parsed) ? parsed : (DayOfWeek?)null)
            .Where(day => day.HasValue)
            .Select(day => day!.Value)
            .Distinct()
            .OrderBy(day => (int)day)
            .ToList();
    }

    private static string BuildStackableHabitLabel(string title, HabitFrequencyType frequencyType, TimeSpan? suggestedTime)
    {
        var label = $"{title} - {frequencyType.ToDisplayName()}";
        return suggestedTime.HasValue ? $"{label} às {suggestedTime.Value:hh\\:mm}" : label;
    }

    private static string? BuildStackBaseKey(int? stackedAfterHabitId, int? stackedAfterSimpleHabitId)
    {
        if (stackedAfterHabitId.HasValue)
        {
            return $"habit:{stackedAfterHabitId.Value}";
        }

        if (stackedAfterSimpleHabitId.HasValue)
        {
            return $"simple:{stackedAfterSimpleHabitId.Value}";
        }

        return null;
    }

    private static string NormalizeColor(string? color)
    {
        return string.IsNullOrWhiteSpace(color) ? HabitVisualOptions.DefaultColor : color.Trim();
    }

    private static string NormalizeIcon(string? icon)
    {
        return HabitVisualOptions.IsAllowedIcon(icon) ? icon!.Trim() : HabitVisualOptions.DefaultIcon;
    }
}
