using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.ViewModels.Habits;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class HabitLocationService
{
    private readonly ApplicationDbContext _dbContext;

    public HabitLocationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId)
    {
        return await _dbContext.HabitLocations
            .AsNoTracking()
            .Where(location => location.UserId == userId)
            .OrderBy(location => location.Name)
            .Select(location => new SelectOptionViewModel
            {
                Value = location.Id.ToString(),
                Text = location.Name
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsForUserAsync(string userId, int locationId)
    {
        return await _dbContext.HabitLocations
            .AsNoTracking()
            .AnyAsync(location => location.Id == locationId && location.UserId == userId);
    }

    public async Task<(bool Success, string? Error, SelectOptionViewModel? Option)> CreateAsync(string userId, HabitLocationCreateViewModel viewModel)
    {
        var name = viewModel.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Informe o nome do local.", null);
        }

        var existingLocation = await _dbContext.HabitLocations
            .AsNoTracking()
            .Where(location => location.UserId == userId && location.Name == name)
            .Select(location => new SelectOptionViewModel
            {
                Value = location.Id.ToString(),
                Text = location.Name
            })
            .FirstOrDefaultAsync();

        if (existingLocation is not null)
        {
            return (true, null, existingLocation);
        }

        var location = new HabitLocation
        {
            UserId = userId,
            Name = name
        };

        _dbContext.HabitLocations.Add(location);
        await _dbContext.SaveChangesAsync();

        return (true, null, new SelectOptionViewModel
        {
            Value = location.Id.ToString(),
            Text = location.Name
        });
    }
}
