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
        var historyStart = today.AddDays(-89);

        var habits = await _dbContext.Habits
            .AsNoTracking()
            .Include(habit => habit.Logs.Where(log => log.Date >= historyStart && log.Date <= today))
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
                    SuggestedTime = habit.SuggestedTime,
                    Icon = habit.Icon,
                    Color = habit.Color,
                    TodayStatus = todayLog?.Status
                };
            })
            .ToList();

        var dueToday = todayHabits.Count;
        var completedToday = todayHabits.Count(habit => habit.TodayStatus == HabitLogStatus.Completed);
        var failedToday = todayHabits.Count(habit => habit.TodayStatus == HabitLogStatus.Failed);
        var skippedToday = todayHabits.Count(habit => habit.TodayStatus == HabitLogStatus.Skipped);
        var completionRate = CalculateRate(completedToday, dueToday);

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
        var weeklyDueSlots = CountDueHabitSlots(habits, weekStart, today);
        var completedLast7Days = weeklyProgress.Sum(point => point.Completed);
        var weeklyCompletionRate = CalculateRate(completedLast7Days, weeklyDueSlots);
        var checkInsLast7Days = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .CountAsync(checkIn => checkIn.UserId == userId && checkIn.Date >= weekStart && checkIn.Date <= today);
        var streak = CalculateCurrentStreak(habits, today);
        var bestStreak = CalculateBestStreak(habits, historyStart, today);
        var betterIndex = Math.Clamp(
            completedToday * 12 + completedLast7Days * 4 + checkInsLast7Days * 5 + streak * 7 + activeIdentities * 3,
            0,
            999);

        return new DashboardViewModel
        {
            TodayCompletionRate = completionRate,
            CompletedToday = completedToday,
            FailedToday = failedToday,
            SkippedToday = skippedToday,
            DueToday = dueToday,
            WeeklyCompletionRate = weeklyCompletionRate,
            CompletedLast7Days = completedLast7Days,
            CheckInsLast7Days = checkInsLast7Days,
            CurrentStreak = streak,
            BestStreak = bestStreak,
            ActiveHabits = habits.Count,
            ActiveGoals = activeGoals,
            ActiveIdentities = activeIdentities,
            BetterIndex = betterIndex,
            FocusIdentityName = focusIdentity?.Name,
            FocusIdentityStatement = focusIdentity?.IdentityStatement,
            TodayHabits = todayHabits,
            WeeklyProgress = weeklyProgress,
            Alerts = BuildAlerts(habits, today)
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

    private static int CountDueHabitSlots(IReadOnlyCollection<Habit> habits, DateTime start, DateTime end)
    {
        var count = 0;

        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            count += habits.Count(habit => IsDueOn(habit, date));
        }

        return count;
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

    private static int CalculateBestStreak(IReadOnlyCollection<Habit> habits, DateTime start, DateTime end)
    {
        var best = 0;
        var current = 0;

        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
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
                current = 0;
                continue;
            }

            current++;
            best = Math.Max(best, current);
        }

        return best;
    }

    private static IReadOnlyList<string> BuildAlerts(IReadOnlyCollection<Habit> habits, DateTime today)
    {
        if (habits.Count == 0)
        {
            return ["Crie o primeiro habito para o dashboard sair do zero."];
        }

        var alerts = new List<string>();
        var weekStart = today.Date.AddDays(-6);

        foreach (var habit in habits.OrderBy(habit => habit.Title))
        {
            var recentLogs = habit.Logs
                .Where(log => log.Date >= weekStart && log.Date <= today)
                .OrderByDescending(log => log.Date)
                .ToList();

            var lastTwoLogs = recentLogs.Take(2).ToList();
            if (lastTwoLogs.Count == 2 && lastTwoLogs.All(log => log.Status == HabitLogStatus.Failed))
            {
                alerts.Add($"O habito \"{habit.Title}\" falhou nas ultimas 2 marcacoes. Reduza a versao de 2 minutos hoje.");
                continue;
            }

            if (recentLogs.Count >= 3)
            {
                var completionRate = CalculateRate(
                    recentLogs.Count(log => log.Status == HabitLogStatus.Completed),
                    recentLogs.Count);

                if (completionRate < 50)
                {
                    alerts.Add($"O habito \"{habit.Title}\" esta com {completionRate}% de conclusao nos ultimos 7 dias.");
                }
            }

            if (alerts.Count == 3)
            {
                break;
            }
        }

        return alerts.Take(3).ToList();
    }

    private static int CalculateRate(int completed, int total)
    {
        return total == 0
            ? 0
            : (int)Math.Round(completed * 100.0 / total);
    }

    private static bool IsDueOn(Habit habit, DateTime date)
    {
        if (date.Date < habit.CreatedAt.Date)
        {
            return false;
        }

        return habit.FrequencyType switch
        {
            HabitFrequencyType.Daily => true,
            HabitFrequencyType.Weekly => date.DayOfWeek == habit.CreatedAt.DayOfWeek,
            HabitFrequencyType.Monthly => date.Day == habit.CreatedAt.Day,
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
