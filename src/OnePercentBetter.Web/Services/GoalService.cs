using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Goals;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class GoalService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryService _categoryService;
    private readonly IdentityService _identityService;

    public GoalService(
        ApplicationDbContext dbContext,
        CategoryService categoryService,
        IdentityService identityService)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<GoalListItemViewModel>> GetListAsync(string userId)
    {
        return await _dbContext.Goals
            .AsNoTracking()
            .Where(goal => goal.UserId == userId)
            .OrderByDescending(goal => goal.CreatedAt)
            .Select(goal => new GoalListItemViewModel
            {
                Id = goal.Id,
                Title = goal.Title,
                Description = goal.Description,
                IdentityName = goal.Identity != null ? goal.Identity.Name : null,
                CategoryName = goal.Category != null ? goal.Category.Name : null,
                Status = goal.Status,
                Priority = goal.Priority,
                StartDate = goal.StartDate,
                TargetDate = goal.TargetDate,
                Color = goal.Color,
                Icon = goal.Icon,
                HabitsCount = goal.Habits.Count
            })
            .ToListAsync();
    }

    public async Task<GoalFormViewModel> CreateFormAsync(string userId)
    {
        return await FillOptionsAsync(new GoalFormViewModel(), userId);
    }

    public async Task<GoalFormViewModel?> EditFormAsync(string userId, int id)
    {
        var goal = await _dbContext.Goals
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (goal is null)
        {
            return null;
        }

        return await FillOptionsAsync(new GoalFormViewModel
        {
            Id = goal.Id,
            Title = goal.Title,
            Description = goal.Description,
            IdentityId = goal.IdentityId,
            CategoryId = goal.CategoryId,
            Status = goal.Status,
            Priority = goal.Priority,
            StartDate = goal.StartDate,
            TargetDate = goal.TargetDate,
            Color = goal.Color,
            Icon = goal.Icon
        }, userId);
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId)
    {
        return await _dbContext.Goals
            .AsNoTracking()
            .Where(goal => goal.UserId == userId)
            .OrderBy(goal => goal.Title)
            .Select(goal => new SelectOptionViewModel
            {
                Value = goal.Id.ToString(),
                Text = goal.Title
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsForUserAsync(string userId, int goalId)
    {
        return await _dbContext.Goals
            .AsNoTracking()
            .AnyAsync(goal => goal.Id == goalId && goal.UserId == userId);
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, GoalFormViewModel viewModel)
    {
        var errors = new Dictionary<string, string>();

        if (viewModel.CategoryId.HasValue && !await _categoryService.ExistsForUserAsync(userId, viewModel.CategoryId.Value))
        {
            errors[nameof(viewModel.CategoryId)] = "Categoria inválida para este usuário.";
        }

        if (viewModel.IdentityId.HasValue && !await _identityService.ExistsForUserAsync(userId, viewModel.IdentityId.Value))
        {
            errors[nameof(viewModel.IdentityId)] = "Identidade inválida para este usuário.";
        }

        if (viewModel.TargetDate.HasValue && viewModel.TargetDate.Value.Date < viewModel.StartDate.Date)
        {
            errors[nameof(viewModel.TargetDate)] = "A data alvo não pode ser anterior à data de início.";
        }

        return errors;
    }

    public async Task<int> CreateAsync(string userId, GoalFormViewModel viewModel)
    {
        var goal = new Goal
        {
            UserId = userId,
            Title = viewModel.Title.Trim(),
            Description = viewModel.Description?.Trim(),
            IdentityId = viewModel.IdentityId,
            CategoryId = viewModel.CategoryId,
            Status = viewModel.Status,
            Priority = viewModel.Priority,
            StartDate = viewModel.StartDate.Date,
            TargetDate = viewModel.TargetDate?.Date,
            Color = NormalizeColor(viewModel.Color),
            Icon = NormalizeIcon(viewModel.Icon)
        };

        _dbContext.Goals.Add(goal);
        await _dbContext.SaveChangesAsync();

        return goal.Id;
    }

    public async Task<bool> UpdateAsync(string userId, GoalFormViewModel viewModel)
    {
        if (viewModel.Id is null)
        {
            return false;
        }

        var goal = await _dbContext.Goals
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == viewModel.Id.Value);

        if (goal is null)
        {
            return false;
        }

        goal.Title = viewModel.Title.Trim();
        goal.Description = viewModel.Description?.Trim();
        goal.IdentityId = viewModel.IdentityId;
        goal.CategoryId = viewModel.CategoryId;
        goal.Status = viewModel.Status;
        goal.Priority = viewModel.Priority;
        goal.StartDate = viewModel.StartDate.Date;
        goal.TargetDate = viewModel.TargetDate?.Date;
        goal.Color = NormalizeColor(viewModel.Color);
        goal.Icon = NormalizeIcon(viewModel.Icon);
        goal.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(string userId, int id, ItemStatus status)
    {
        var goal = await _dbContext.Goals
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (goal is null)
        {
            return false;
        }

        goal.Status = status;
        goal.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(string userId, int id)
    {
        var goal = await _dbContext.Goals
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (goal is null)
        {
            return false;
        }

        _dbContext.Goals.Remove(goal);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<GoalFormViewModel> FillOptionsAsync(GoalFormViewModel viewModel, string userId)
    {
        viewModel.Categories = await _categoryService.GetOptionsAsync(userId);
        viewModel.Identities = await _identityService.GetOptionsAsync(userId);
        return viewModel;
    }

    private static string NormalizeColor(string? color)
    {
        return string.IsNullOrWhiteSpace(color) ? "#38bdf8" : color.Trim();
    }

    private static string NormalizeIcon(string? icon)
    {
        return string.IsNullOrWhiteSpace(icon) ? "target" : icon.Trim();
    }
}
