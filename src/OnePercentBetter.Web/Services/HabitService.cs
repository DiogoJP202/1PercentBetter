using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
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

    public HabitService(
        ApplicationDbContext dbContext,
        CategoryService categoryService,
        IdentityService identityService,
        GoalService goalService)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
        _identityService = identityService;
        _goalService = goalService;
    }

    public async Task<IReadOnlyList<HabitListItemViewModel>> GetListAsync(string userId)
    {
        var today = DateTime.Today;

        return await _dbContext.Habits
            .AsNoTracking()
            .Where(habit => habit.UserId == userId)
            .OrderByDescending(habit => habit.CreatedAt)
            .Select(habit => new HabitListItemViewModel
            {
                Id = habit.Id,
                Title = habit.Title,
                TwoMinuteVersion = habit.TwoMinuteVersion,
                Trigger = habit.Trigger,
                IdentityName = habit.Identity != null ? habit.Identity.Name : null,
                GoalTitle = habit.Goal != null ? habit.Goal.Title : null,
                CategoryName = habit.Category != null ? habit.Category.Name : null,
                Status = habit.Status,
                TodayStatus = habit.Logs
                    .Where(log => log.Date == today)
                    .Select(log => (HabitLogStatus?)log.Status)
                    .FirstOrDefault(),
                Color = habit.Color,
                Icon = habit.Icon
            })
            .ToListAsync();
    }

    public async Task<HabitFormViewModel> CreateFormAsync(string userId)
    {
        return await FillOptionsAsync(new HabitFormViewModel(), userId);
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

        return await FillOptionsAsync(new HabitFormViewModel
        {
            Id = habit.Id,
            Title = habit.Title,
            Description = habit.Description,
            IdentityId = habit.IdentityId,
            GoalId = habit.GoalId,
            CategoryId = habit.CategoryId,
            FrequencyType = habit.FrequencyType,
            DaysOfWeek = habit.DaysOfWeek,
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
            FrequencyType = viewModel.FrequencyType,
            DaysOfWeek = viewModel.DaysOfWeek,
            SuggestedTime = viewModel.SuggestedTime,
            Difficulty = viewModel.Difficulty,
            TwoMinuteVersion = viewModel.TwoMinuteVersion.Trim(),
            Trigger = viewModel.Trigger.Trim(),
            Reward = viewModel.Reward?.Trim(),
            Status = viewModel.Status,
            Color = viewModel.Color,
            Icon = viewModel.Icon
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
        habit.FrequencyType = viewModel.FrequencyType;
        habit.DaysOfWeek = viewModel.DaysOfWeek;
        habit.SuggestedTime = viewModel.SuggestedTime;
        habit.Difficulty = viewModel.Difficulty;
        habit.TwoMinuteVersion = viewModel.TwoMinuteVersion.Trim();
        habit.Trigger = viewModel.Trigger.Trim();
        habit.Reward = viewModel.Reward?.Trim();
        habit.Status = viewModel.Status;
        habit.Color = viewModel.Color;
        habit.Icon = viewModel.Icon;
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

    private async Task<HabitFormViewModel> FillOptionsAsync(HabitFormViewModel viewModel, string userId)
    {
        viewModel.Categories = await _categoryService.GetOptionsAsync(userId);
        viewModel.Identities = await _identityService.GetOptionsAsync(userId);
        viewModel.Goals = await _goalService.GetOptionsAsync(userId);
        return viewModel;
    }
}
