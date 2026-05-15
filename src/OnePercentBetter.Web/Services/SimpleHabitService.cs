using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.ViewModels.Habits;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class SimpleHabitService
{
    private readonly ApplicationDbContext _dbContext;

    public SimpleHabitService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId)
    {
        return await _dbContext.SimpleHabits
            .AsNoTracking()
            .Where(simpleHabit => simpleHabit.UserId == userId && simpleHabit.IsActive)
            .OrderBy(simpleHabit => simpleHabit.ScheduledTime)
            .ThenBy(simpleHabit => simpleHabit.Name)
            .Select(simpleHabit => new SelectOptionViewModel
            {
                Value = simpleHabit.Id.ToString(),
                Text = BuildLabel(simpleHabit.Name, simpleHabit.ScheduledTime)
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsForUserAsync(string userId, int simpleHabitId)
    {
        return await _dbContext.SimpleHabits
            .AsNoTracking()
            .AnyAsync(simpleHabit => simpleHabit.Id == simpleHabitId && simpleHabit.UserId == userId && simpleHabit.IsActive);
    }

    public async Task<(bool Success, string? Error, SelectOptionViewModel? Option)> CreateAsync(string userId, SimpleHabitCreateViewModel viewModel)
    {
        var name = viewModel.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Informe o nome do hábito simples.", null);
        }

        var existing = await _dbContext.SimpleHabits
            .AsNoTracking()
            .Where(simpleHabit =>
                simpleHabit.UserId == userId
                && simpleHabit.IsActive
                && simpleHabit.Name == name
                && simpleHabit.ScheduledTime == viewModel.ScheduledTime)
            .Select(simpleHabit => new SelectOptionViewModel
            {
                Value = simpleHabit.Id.ToString(),
                Text = BuildLabel(simpleHabit.Name, simpleHabit.ScheduledTime)
            })
            .FirstOrDefaultAsync();

        if (existing is not null)
        {
            return (true, null, existing);
        }

        var simpleHabit = new SimpleHabit
        {
            UserId = userId,
            Name = name,
            ScheduledTime = viewModel.ScheduledTime
        };

        _dbContext.SimpleHabits.Add(simpleHabit);
        await _dbContext.SaveChangesAsync();

        return (true, null, new SelectOptionViewModel
        {
            Value = simpleHabit.Id.ToString(),
            Text = BuildLabel(simpleHabit.Name, simpleHabit.ScheduledTime)
        });
    }

    public static string BuildLabel(string name, TimeSpan? scheduledTime)
    {
        return scheduledTime.HasValue ? $"{name} às {scheduledTime.Value:hh\\:mm}" : name;
    }
}
