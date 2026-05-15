using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Identity;
using OnePercentBetter.Web.ViewModels.Onboarding;

namespace OnePercentBetter.Web.Services;

public class OnboardingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryService _categoryService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OnboardingService(
        ApplicationDbContext dbContext,
        CategoryService categoryService,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
        _userManager = userManager;
    }

    public async Task<bool> IsCompletedAsync(string userId)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId && user.OnboardingCompletedAt != null);
    }

    public async Task<OnboardingViewModel> CreateFormAsync(string userId)
    {
        var categories = await _categoryService.GetOptionsAsync(userId);

        return new OnboardingViewModel
        {
            Categories = categories
        };
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, OnboardingViewModel viewModel)
    {
        var errors = new Dictionary<string, string>();

        if (viewModel.CategoryId.HasValue && !await _categoryService.ExistsForUserAsync(userId, viewModel.CategoryId.Value))
        {
            errors[nameof(viewModel.CategoryId)] = "Área de foco inválida para este usuário.";
        }

        return errors;
    }

    public async Task CompleteAsync(string userId, OnboardingViewModel viewModel)
    {
        var categoryId = viewModel.CategoryId
            ?? throw new InvalidOperationException("Focus area is required to complete onboarding.");

        var identity = new UserIdentity
        {
            UserId = userId,
            Name = viewModel.IdentityName.Trim(),
            IdentityStatement = viewModel.IdentityStatement.Trim(),
            CategoryId = categoryId,
            Color = "#22c55e",
            Icon = "user-round-check"
        };

        var goal = new Goal
        {
            UserId = userId,
            Identity = identity,
            CategoryId = categoryId,
            Title = viewModel.GoalTitle.Trim(),
            Color = "#38bdf8",
            Icon = "target"
        };

        var habit = new Habit
        {
            UserId = userId,
            Identity = identity,
            Goal = goal,
            CategoryId = categoryId,
            Title = viewModel.HabitTitle.Trim(),
            TwoMinuteVersion = viewModel.TwoMinuteVersion.Trim(),
            Trigger = viewModel.Trigger.Trim(),
            Reward = viewModel.Reward?.Trim()
        };

        _dbContext.UserIdentities.Add(identity);
        _dbContext.Goals.Add(goal);
        _dbContext.Habits.Add(habit);

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("Authenticated user was not found.");
        user.OnboardingCompletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}
