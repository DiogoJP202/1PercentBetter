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

    public async Task<(bool Success, string? Error, SelectOptionViewModel? Option)> UpdateAsync(string userId, SimpleHabitCreateViewModel viewModel)
    {
        if (!viewModel.Id.HasValue || viewModel.Id.Value <= 0)
        {
            return (false, "Informe um hábito simples válido para edição.", null);
        }

        var simpleHabit = await _dbContext.SimpleHabits
            .FirstOrDefaultAsync(item => item.Id == viewModel.Id.Value && item.UserId == userId && item.IsActive);

        if (simpleHabit is null)
        {
            return (false, "Hábito simples não encontrado para este usuário.", null);
        }

        var name = viewModel.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Informe o nome do hábito simples.", null);
        }

        var duplicate = await _dbContext.SimpleHabits
            .AsNoTracking()
            .Where(item =>
                item.UserId == userId
                && item.IsActive
                && item.Id != simpleHabit.Id
                && item.Name == name
                && item.ScheduledTime == viewModel.ScheduledTime)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.Id.ToString(),
                Text = BuildLabel(item.Name, item.ScheduledTime)
            })
            .FirstOrDefaultAsync();

        if (duplicate is not null)
        {
            return (false, "Já existe um hábito simples igual com esse horário.", null);
        }

        simpleHabit.Name = name;
        simpleHabit.ScheduledTime = viewModel.ScheduledTime;
        simpleHabit.UpdatedAt = DateTime.UtcNow;

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

