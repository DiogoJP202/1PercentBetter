using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Dashboard;

namespace OnePercentBetter.Web.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(string userId)
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-6);

        var habits = await _dbContext.Habits
            .AsNoTracking()
            .Include(habit => habit.Logs.Where(log => log.Date >= weekStart && log.Date <= today))
            .Where(habit => habit.UserId == userId && habit.Status == ItemStatus.Active)
            .OrderBy(habit => habit.Title)
            .ToListAsync();

        var todayHabits = habits
            .Where(habit => IsDueOn(habit, today))
            .Select(habit =>
            {
                var todayLog = habit.Logs.FirstOrDefault(log => log.Date == today);
                return new TodayHabitViewModel
                {
                    Id = habit.Id,
                    Title = habit.Title,
                    TwoMinuteVersion = habit.TwoMinuteVersion,
                    Trigger = habit.Trigger,
                    Icon = habit.Icon,
                    Color = habit.Color,
                    TodayStatus = todayLog?.Status
                };
            })
            .ToList();

        var completedToday = todayHabits.Count(habit => habit.TodayStatus == HabitLogStatus.Completed);
        var completionRate = todayHabits.Count == 0
            ? 0
            : (int)Math.Round(completedToday * 100.0 / todayHabits.Count);

        var activeGoals = await _dbContext.Goals
            .AsNoTracking()
            .CountAsync(goal => goal.UserId == userId && goal.Status == ItemStatus.Active);

        var activeIdentities = await _dbContext.UserIdentities
            .AsNoTracking()
            .CountAsync(identity => identity.UserId == userId && identity.Status == ItemStatus.Active);

        var focusIdentity = await _dbContext.UserIdentities
            .AsNoTracking()
            .Where(identity => identity.UserId == userId && identity.Status == ItemStatus.Active)
            .OrderByDescending(identity => identity.Habits.Count)
            .FirstOrDefaultAsync();

        var weeklyProgress = BuildWeeklyProgress(habits, weekStart, today);
        var streak = CalculateCurrentStreak(habits, today);
        var betterIndex = completedToday * 10 + streak * 5;

        return new DashboardViewModel
        {
            TodayCompletionRate = completionRate,
            CurrentStreak = streak,
            BestStreak = streak,
            ActiveHabits = habits.Count,
            ActiveGoals = activeGoals,
            ActiveIdentities = activeIdentities,
            BetterIndex = betterIndex,
            FocusIdentityName = focusIdentity?.Name,
            FocusIdentityStatement = focusIdentity?.IdentityStatement,
            TodayHabits = todayHabits,
            WeeklyProgress = weeklyProgress
        };
    }

    public async Task<IReadOnlyList<WeeklyProgressPointViewModel>> GetWeeklyProgressAsync(string userId)
    {
        var dashboard = await GetDashboardAsync(userId);
        return dashboard.WeeklyProgress;
    }

    private static IReadOnlyList<WeeklyProgressPointViewModel> BuildWeeklyProgress(
        IReadOnlyCollection<Habit> habits,
        DateTime start,
        DateTime end)
    {
        var points = new List<WeeklyProgressPointViewModel>();

        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            var logs = habits.SelectMany(habit => habit.Logs).Where(log => log.Date == date).ToList();
            points.Add(new WeeklyProgressPointViewModel
            {
                Label = date.ToString("dd/MM"),
                Completed = logs.Count(log => log.Status == HabitLogStatus.Completed),
                Failed = logs.Count(log => log.Status == HabitLogStatus.Failed),
                Skipped = logs.Count(log => log.Status == HabitLogStatus.Skipped)
            });
        }

        return points;
    }

    private static int CalculateCurrentStreak(IReadOnlyCollection<Habit> habits, DateTime today)
    {
        var streak = 0;

        for (var date = today.Date; date >= today.Date.AddDays(-60); date = date.AddDays(-1))
        {
            var dueHabits = habits.Where(habit => IsDueOn(habit, date)).ToList();
            if (dueHabits.Count == 0)
            {
                continue;
            }

            var completedAll = dueHabits.All(habit =>
                habit.Logs.Any(log => log.Date == date && log.Status == HabitLogStatus.Completed));

            if (!completedAll)
            {
                break;
            }

            streak++;
        }

        return streak;
    }

    private static bool IsDueOn(Habit habit, DateTime date)
    {
        return habit.FrequencyType switch
        {
            HabitFrequencyType.Daily => true,
            HabitFrequencyType.Weekly => date.DayOfWeek == habit.CreatedAt.DayOfWeek,
            HabitFrequencyType.SpecificDays => IsSpecificDayDue(habit.DaysOfWeek, date.DayOfWeek),
            _ => true
        };
    }

    private static bool IsSpecificDayDue(string? daysOfWeek, DayOfWeek dayOfWeek)
    {
        if (string.IsNullOrWhiteSpace(daysOfWeek))
        {
            return false;
        }

        return daysOfWeek
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(day => Enum.TryParse<DayOfWeek>(day, true, out var parsed) && parsed == dayOfWeek);
    }
}
