using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.ViewModels.Identities;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class IdentityService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryService _categoryService;

    public IdentityService(ApplicationDbContext dbContext, CategoryService categoryService)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
    }

    public async Task<IReadOnlyList<IdentityListItemViewModel>> GetListAsync(string userId)
    {
        return await _dbContext.UserIdentities
            .AsNoTracking()
            .Where(identity => identity.UserId == userId)
            .OrderByDescending(identity => identity.CreatedAt)
            .Select(identity => new IdentityListItemViewModel
            {
                Id = identity.Id,
                Name = identity.Name,
                IdentityStatement = identity.IdentityStatement,
                CategoryName = identity.Category != null ? identity.Category.Name : null,
                Status = identity.Status,
                Color = identity.Color,
                Icon = identity.Icon,
                HabitsCount = identity.Habits.Count,
                GoalsCount = identity.Goals.Count
            })
            .ToListAsync();
    }

    public async Task<IdentityFormViewModel> CreateFormAsync(string userId)
    {
        return new IdentityFormViewModel
        {
            Categories = await _categoryService.GetOptionsAsync(userId)
        };
    }

    public async Task<IdentityFormViewModel?> EditFormAsync(string userId, int id)
    {
        var identity = await _dbContext.UserIdentities
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (identity is null)
        {
            return null;
        }

        return new IdentityFormViewModel
        {
            Id = identity.Id,
            Name = identity.Name,
            IdentityStatement = identity.IdentityStatement,
            Description = identity.Description,
            CategoryId = identity.CategoryId,
            Status = identity.Status,
            Color = identity.Color,
            Icon = identity.Icon,
            Categories = await _categoryService.GetOptionsAsync(userId)
        };
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId)
    {
        return await _dbContext.UserIdentities
            .AsNoTracking()
            .Where(identity => identity.UserId == userId)
            .OrderBy(identity => identity.Name)
            .Select(identity => new SelectOptionViewModel
            {
                Value = identity.Id.ToString(),
                Text = identity.Name
            })
            .ToListAsync();
    }

    public async Task<int> CreateAsync(string userId, IdentityFormViewModel viewModel)
    {
        var identity = new UserIdentity
        {
            UserId = userId,
            Name = viewModel.Name.Trim(),
            IdentityStatement = viewModel.IdentityStatement.Trim(),
            Description = viewModel.Description?.Trim(),
            CategoryId = viewModel.CategoryId,
            Status = viewModel.Status,
            Color = viewModel.Color,
            Icon = viewModel.Icon
        };

        _dbContext.UserIdentities.Add(identity);
        await _dbContext.SaveChangesAsync();

        return identity.Id;
    }

    public async Task<bool> UpdateAsync(string userId, IdentityFormViewModel viewModel)
    {
        if (viewModel.Id is null)
        {
            return false;
        }

        var identity = await _dbContext.UserIdentities
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == viewModel.Id.Value);

        if (identity is null)
        {
            return false;
        }

        identity.Name = viewModel.Name.Trim();
        identity.IdentityStatement = viewModel.IdentityStatement.Trim();
        identity.Description = viewModel.Description?.Trim();
        identity.CategoryId = viewModel.CategoryId;
        identity.Status = viewModel.Status;
        identity.Color = viewModel.Color;
        identity.Icon = viewModel.Icon;
        identity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }
}
