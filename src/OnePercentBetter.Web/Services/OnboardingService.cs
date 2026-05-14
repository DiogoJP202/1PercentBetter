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
            CategoryId = int.TryParse(categories.FirstOrDefault()?.Value, out var categoryId) ? categoryId : 0,
            Categories = categories
        };
    }

    public async Task CompleteAsync(string userId, OnboardingViewModel viewModel)
    {
        var identity = new UserIdentity
        {
            UserId = userId,
            Name = viewModel.IdentityName.Trim(),
            IdentityStatement = viewModel.IdentityStatement.Trim(),
            CategoryId = viewModel.CategoryId,
            Color = "#22c55e",
            Icon = "user-round-check"
        };

        var goal = new Goal
        {
            UserId = userId,
            Identity = identity,
            CategoryId = viewModel.CategoryId,
            Title = viewModel.GoalTitle.Trim(),
            Color = "#38bdf8",
            Icon = "target"
        };

        var habit = new Habit
        {
            UserId = userId,
            Identity = identity,
            Goal = goal,
            CategoryId = viewModel.CategoryId,
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
