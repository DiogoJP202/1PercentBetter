using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
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
                Description = identity.Description,
                CategoryName = identity.Category != null ? identity.Category.Name : null,
                Status = identity.Status,
                Color = identity.Color,
                Icon = identity.Icon,
                HabitsCount = identity.Habits.Count,
                GoalsCount = identity.Goals.Count,
                TasksCount = identity.TaskItems.Count,
                PendingTasksCount = identity.TaskItems.Count(taskItem =>
                    taskItem.Status == TaskItemStatus.Pending
                    || taskItem.Status == TaskItemStatus.InProgress
                    || taskItem.Status == TaskItemStatus.Postponed)
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

    public async Task<bool> ExistsForUserAsync(string userId, int identityId)
    {
        return await _dbContext.UserIdentities
            .AsNoTracking()
            .AnyAsync(identity => identity.Id == identityId && identity.UserId == userId);
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, IdentityFormViewModel viewModel)
    {
        var errors = new Dictionary<string, string>();

        if (viewModel.CategoryId.HasValue && !await _categoryService.ExistsForUserAsync(userId, viewModel.CategoryId.Value))
        {
            errors[nameof(viewModel.CategoryId)] = "Categoria inválida para este usuário.";
        }

        return errors;
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
            Color = NormalizeColor(viewModel.Color),
            Icon = NormalizeIcon(viewModel.Icon)
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
        identity.Color = NormalizeColor(viewModel.Color);
        identity.Icon = NormalizeIcon(viewModel.Icon);
        identity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string userId, int id)
    {
        var identity = await _dbContext.UserIdentities
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (identity is null)
        {
            return false;
        }

        _dbContext.UserIdentities.Remove(identity);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static string NormalizeColor(string? color)
    {
        return string.IsNullOrWhiteSpace(color) ? "#22c55e" : color.Trim();
    }

    private static string NormalizeIcon(string? icon)
    {
        return string.IsNullOrWhiteSpace(icon) ? "user-round-check" : icon.Trim();
    }
}
