using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Extensions;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Calendar;

namespace OnePercentBetter.Web.Services;

public class CalendarService
{
    public const string ImprovementType = "improvement";
    public const string CommonType = "common";
    public const string CheckInType = "checkin";
    public const string TaskType = "task";

    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");
    private readonly ApplicationDbContext _dbContext;

    public CalendarService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CalendarOverviewViewModel> GetOverviewAsync(string userId)
    {
        var today = AppClock.Today;
        var start = new DateTime(today.Year, today.Month, 1);
        var end = start.AddMonths(1);
        var habits = await GetHabitSourcesAsync(userId);
        var plannedOccurrences = CountPlannedOccurrences(habits, start, end);
        var completedLogs = await _dbContext.HabitLogs
            .AsNoTracking()
            .CountAsync(log => log.UserId == userId
                && log.Date >= start
                && log.Date < end
                && log.Status == HabitLogStatus.Completed);
        var daysWithCheckIn = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .CountAsync(checkIn => checkIn.UserId == userId && checkIn.Date >= start && checkIn.Date < end);
        var activeSimpleHabits = await _dbContext.SimpleHabits
            .AsNoTracking()
            .CountAsync(simpleHabit => simpleHabit.UserId == userId && simpleHabit.IsActive);
        var plannedTasks = await _dbContext.TaskItems
            .AsNoTracking()
            .CountAsync(taskItem => taskItem.UserId == userId
                && taskItem.TaskDate.HasValue
                && taskItem.TaskDate.Value >= start
                && taskItem.TaskDate.Value < end);
        var completedTasks = await _dbContext.TaskItems
            .AsNoTracking()
            .CountAsync(taskItem => taskItem.UserId == userId
                && taskItem.Status == TaskItemStatus.Completed
                && taskItem.TaskDate.HasValue
                && taskItem.TaskDate.Value >= start
                && taskItem.TaskDate.Value < end);
        var overdueTasks = await _dbContext.TaskItems
            .AsNoTracking()
            .CountAsync(taskItem => taskItem.UserId == userId
                && (taskItem.Status == TaskItemStatus.Pending
                    || taskItem.Status == TaskItemStatus.InProgress
                    || taskItem.Status == TaskItemStatus.Postponed)
                && ((taskItem.DueDate.HasValue && taskItem.DueDate.Value < today)
                    || (taskItem.TaskDate.HasValue && taskItem.TaskDate.Value < today)));

        return new CalendarOverviewViewModel
        {
            MonthLabel = start.ToString("MMMM 'de' yyyy", PtBr),
            CompletedHabitLogs = completedLogs,
            PlannedHabitOccurrences = plannedOccurrences,
            ConsistencyRate = plannedOccurrences == 0 ? 0 : (int)Math.Round(completedLogs * 100d / plannedOccurrences),
            DaysWithCheckIn = daysWithCheckIn,
            CurrentCheckInStreak = await GetCurrentCheckInStreakAsync(userId),
            ActiveSimpleHabits = activeSimpleHabits,
            PlannedTasks = plannedTasks,
            CompletedTasks = completedTasks,
            OverdueTasks = overdueTasks
        };
    }

    public async Task<IReadOnlyList<CalendarEventViewModel>> GetEventsAsync(
        string userId,
        DateTime? start,
        DateTime? end,
        IReadOnlySet<string> selectedTypes)
    {
        var startDate = (start ?? AppClock.Today.AddMonths(-1)).Date;
        var endDate = (end ?? AppClock.Today.AddMonths(1)).Date;
        var events = new List<CalendarEventViewModel>();

        if (IncludesType(selectedTypes, ImprovementType))
        {
            events.AddRange(await GetImprovementHabitEventsAsync(userId, startDate, endDate));
        }

        if (IncludesType(selectedTypes, CommonType))
        {
            events.AddRange(await GetCommonHabitEventsAsync(userId, startDate, endDate));
        }

        if (IncludesType(selectedTypes, CheckInType))
        {
            events.AddRange(await GetCheckInEventsAsync(userId, startDate, endDate));
        }

        if (IncludesType(selectedTypes, TaskType))
        {
            events.AddRange(await GetTaskEventsAsync(userId, startDate, endDate));
        }

        return events
            .OrderBy(item => item.Start, StringComparer.Ordinal)
            .ThenBy(item => item.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public async Task<CalendarDayDetailViewModel> GetDayDetailsAsync(string userId, DateTime date)
    {
        var targetDate = date.Date;
        var habits = await GetHabitSourcesAsync(userId);
        var logs = await GetLogMapAsync(userId, targetDate, targetDate.AddDays(1));
        var simpleHabits = await GetSimpleHabitSourcesAsync(userId);
        var checkIn = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .Where(item => item.UserId == userId && item.Date == targetDate)
            .Select(item => new
            {
                item.Id,
                item.Mood,
                item.DayScore,
                item.EnergyLevel,
                item.ProductivityLevel,
                item.SmallWin,
                item.MainDifficulty
            })
            .FirstOrDefaultAsync();

        var improvementHabits = habits
            .Where(habit => AppliesOn(habit, targetDate))
            .Select(habit =>
            {
                logs.TryGetValue((habit.Id, targetDate), out var log);
                var status = log?.Status;

                return new CalendarDayHabitViewModel
                {
                    Id = habit.Id,
                    Title = habit.Title,
                    Color = habit.Color,
                    Icon = habit.Icon,
                    SuggestedTime = FormatHabitTimeRange(habit.SuggestedTime, habit.EndTime),
                    IdentityName = habit.IdentityName,
                    GoalTitle = habit.GoalTitle,
                    LocationName = habit.LocationName,
                    Status = status?.ToString() ?? "Pending",
                    StatusLabel = status?.ToDisplayName() ?? "Pendente",
                    StatusTone = GetStatusTone(status),
                    IsCompleted = status == HabitLogStatus.Completed
                };
            })
            .OrderBy(item => item.SuggestedTime)
            .ThenBy(item => item.Title)
            .ToList();

        var commonHabits = simpleHabits
            .Where(simpleHabit => SimpleHabitAppliesOn(simpleHabit, targetDate))
            .Select(simpleHabit => new CalendarDaySimpleHabitViewModel
            {
                Id = simpleHabit.Id,
                Name = simpleHabit.Name,
                ScheduledTime = FormatTime(simpleHabit.ScheduledTime)
            })
            .OrderBy(item => item.ScheduledTime)
            .ThenBy(item => item.Name)
            .ToList();

        var tasks = await _dbContext.TaskItems
            .AsNoTracking()
            .Where(taskItem => taskItem.UserId == userId && taskItem.TaskDate.HasValue && taskItem.TaskDate.Value == targetDate)
            .OrderBy(taskItem => taskItem.StartTime.HasValue ? 0 : 1)
            .ThenBy(taskItem => taskItem.StartTime)
            .ThenByDescending(taskItem => taskItem.Priority)
            .Select(taskItem => new CalendarDayTaskViewModel
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Color = taskItem.Color,
                Icon = taskItem.Icon,
                Status = taskItem.Status.ToString(),
                StatusLabel = taskItem.Status.ToDisplayName(),
                StatusTone = GetTaskStatusTone(taskItem.Status),
                Priority = taskItem.Priority.ToString(),
                PriorityLabel = taskItem.Priority.ToDisplayName(),
                TimeRange = FormatTaskTimeRange(taskItem.StartTime, taskItem.EndTime),
                GoalTitle = taskItem.Goal != null ? taskItem.Goal.Title : null,
                IdentityName = taskItem.Identity != null ? taskItem.Identity.Name : null,
                IsCompleted = taskItem.Status == TaskItemStatus.Completed
            })
            .ToListAsync();

        return new CalendarDayDetailViewModel
        {
            Date = targetDate,
            DateLabel = targetDate.ToString("dddd, dd/MM/yyyy", PtBr),
            PlannedCount = improvementHabits.Count,
            CompletedCount = improvementHabits.Count(item => item.IsCompleted),
            PendingCount = improvementHabits.Count(item => !item.IsCompleted),
            PlannedTasksCount = tasks.Count,
            CompletedTasksCount = tasks.Count(item => item.IsCompleted),
            PendingTasksCount = tasks.Count(item => !item.IsCompleted),
            ImprovementHabits = improvementHabits,
            CommonHabits = commonHabits,
            Tasks = tasks,
            CheckIn = checkIn is null
                ? null
                : new CalendarDayCheckInViewModel
                {
                    Id = checkIn.Id,
                    MoodLabel = checkIn.Mood.ToDisplayName(),
                    MoodFace = GetMoodFace(checkIn.Mood),
                    TotalScore = checkIn.DayScore + checkIn.EnergyLevel + checkIn.ProductivityLevel,
                    SmallWin = checkIn.SmallWin,
                    MainDifficulty = checkIn.MainDifficulty
                }
        };
    }

    public async Task<bool> RegisterHabitStatusAsync(string userId, int habitId, DateTime date, HabitLogStatus status)
    {
        var habitExists = await _dbContext.Habits
            .AnyAsync(habit => habit.UserId == userId && habit.Id == habitId);

        if (!habitExists)
        {
            return false;
        }

        var targetDate = date.Date;
        var log = await _dbContext.HabitLogs
            .FirstOrDefaultAsync(item => item.UserId == userId && item.HabitId == habitId && item.Date == targetDate);

        if (log is null)
        {
            log = new HabitLog
            {
                UserId = userId,
                HabitId = habitId,
                Date = targetDate
            };
            _dbContext.HabitLogs.Add(log);
        }

        log.Status = status;
        log.CompletedAt = status == HabitLogStatus.Completed ? DateTime.UtcNow : null;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RegisterTaskStatusAsync(string userId, int taskItemId, TaskItemStatus status)
    {
        var taskItem = await _dbContext.TaskItems
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == taskItemId);

        if (taskItem is null)
        {
            return false;
        }

        taskItem.Status = status;
        taskItem.CompletedAt = status == TaskItemStatus.Completed ? DateTime.UtcNow : null;
        taskItem.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<IReadOnlyList<CalendarEventViewModel>> GetImprovementHabitEventsAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var habits = await GetHabitSourcesAsync(userId);
        var logs = await GetLogMapAsync(userId, startDate, endDate);
        var events = new List<CalendarEventViewModel>();

        foreach (var date in EachDate(startDate, endDate))
        {
            foreach (var habit in habits.Where(habit => AppliesOn(habit, date)))
            {
                logs.TryGetValue((habit.Id, date), out var log);
                var status = log?.Status;
                var statusKey = status?.ToString() ?? "Pending";
                var statusLabel = status?.ToDisplayName() ?? "Pendente";
                var colors = GetImprovementEventColors(status, habit.Color);

                events.Add(new CalendarEventViewModel
                {
                    Id = $"habit-{habit.Id}-{date:yyyyMMdd}",
                    Title = BuildTimedTitle(habit.Title, habit.SuggestedTime),
                    Start = BuildStart(date, habit.SuggestedTime),
                    End = BuildEnd(date, habit.SuggestedTime, habit.EndTime),
                    AllDay = !habit.SuggestedTime.HasValue,
                    BackgroundColor = colors.Background,
                    BorderColor = colors.Border,
                    TextColor = colors.Text,
                    ClassNames = ["calendar-event", "calendar-event-improvement", $"calendar-event-{statusKey.ToLowerInvariant()}"],
                    ExtendedProps = new CalendarEventExtendedPropsViewModel
                    {
                        Type = ImprovementType,
                        TypeLabel = "HÃ¡bito de melhoria",
                        HabitId = habit.Id,
                        Date = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Status = statusKey,
                        StatusLabel = statusLabel,
                        HabitColor = habit.Color,
                        HabitIcon = habit.Icon,
                        Time = FormatHabitTimeRange(habit.SuggestedTime, habit.EndTime),
                        Notes = log?.Notes
                    }
                });
            }
        }

        return events;
    }

    private async Task<IReadOnlyList<CalendarEventViewModel>> GetCommonHabitEventsAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var simpleHabits = await GetSimpleHabitSourcesAsync(userId);
        var events = new List<CalendarEventViewModel>();

        foreach (var date in EachDate(startDate, endDate))
        {
            foreach (var simpleHabit in simpleHabits.Where(simpleHabit => SimpleHabitAppliesOn(simpleHabit, date)))
            {
                events.Add(new CalendarEventViewModel
                {
                    Id = $"simple-{simpleHabit.Id}-{date:yyyyMMdd}",
                    Title = BuildTimedTitle(simpleHabit.Name, simpleHabit.ScheduledTime),
                    Start = BuildStart(date, simpleHabit.ScheduledTime),
                    AllDay = !simpleHabit.ScheduledTime.HasValue,
                    BackgroundColor = "#1e293b",
                    BorderColor = "#a78bfa",
                    TextColor = "#e2e8f0",
                    ClassNames = ["calendar-event", "calendar-event-common"],
                    ExtendedProps = new CalendarEventExtendedPropsViewModel
                    {
                        Type = CommonType,
                        TypeLabel = "HÃ¡bito comum",
                        SimpleHabitId = simpleHabit.Id,
                        Date = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Status = "Scheduled",
                        StatusLabel = "Planejado",
                        HabitColor = "#a78bfa",
                        HabitIcon = "calendar-clock",
                        Time = FormatTime(simpleHabit.ScheduledTime)
                    }
                });
            }
        }

        return events;
    }

    private async Task<IReadOnlyList<CalendarEventViewModel>> GetCheckInEventsAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var checkIns = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .Where(checkIn => checkIn.UserId == userId && checkIn.Date >= startDate && checkIn.Date < endDate)
            .OrderBy(checkIn => checkIn.Date)
            .Select(checkIn => new
            {
                checkIn.Id,
                checkIn.Date,
                checkIn.Mood,
                checkIn.DayScore,
                checkIn.EnergyLevel,
                checkIn.ProductivityLevel,
                checkIn.Notes
            })
            .ToListAsync();

        return checkIns
            .Select(checkIn =>
            {
                var total = checkIn.DayScore + checkIn.EnergyLevel + checkIn.ProductivityLevel;

                return new CalendarEventViewModel
                {
                    Id = $"checkin-{checkIn.Id}",
                    Title = $"Check-in {total}/15",
                    Start = checkIn.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    BackgroundColor = "#312e81",
                    BorderColor = "#818cf8",
                    TextColor = "#ede9fe",
                    ClassNames = ["calendar-event", "calendar-event-checkin"],
                    ExtendedProps = new CalendarEventExtendedPropsViewModel
                    {
                        Type = CheckInType,
                        TypeLabel = "Check-in",
                        CheckInId = checkIn.Id,
                        Date = checkIn.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Status = "Registered",
                        StatusLabel = $"{GetMoodFace(checkIn.Mood)} {checkIn.Mood.ToDisplayName()}",
                        HabitColor = "#818cf8",
                        HabitIcon = "calendar-check",
                        Notes = checkIn.Notes
                    }
                };
            })
            .ToList();
    }

    private async Task<IReadOnlyList<CalendarEventViewModel>> GetTaskEventsAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var tasks = await _dbContext.TaskItems
            .AsNoTracking()
            .Where(taskItem => taskItem.UserId == userId
                && taskItem.ShowOnCalendar
                && taskItem.TaskDate.HasValue
                && taskItem.TaskDate.Value >= startDate
                && taskItem.TaskDate.Value < endDate)
            .OrderBy(taskItem => taskItem.TaskDate)
            .ThenBy(taskItem => taskItem.StartTime.HasValue ? 0 : 1)
            .ThenBy(taskItem => taskItem.StartTime)
            .ThenByDescending(taskItem => taskItem.Priority)
            .Select(taskItem => new
            {
                taskItem.Id,
                taskItem.Title,
                taskItem.TaskDate,
                taskItem.StartTime,
                taskItem.EndTime,
                taskItem.Status,
                taskItem.Priority,
                taskItem.Color,
                taskItem.Icon,
                taskItem.Notes
            })
            .ToListAsync();

        return tasks.Select(taskItem =>
        {
            var start = taskItem.TaskDate!.Value;
            var hasStart = taskItem.StartTime.HasValue;
            var eventStart = hasStart
                ? start.Add(taskItem.StartTime!.Value)
                : start;
            var eventEnd = taskItem.EndTime.HasValue
                ? start.Add(taskItem.EndTime.Value)
                : (DateTime?)null;
            var (background, border, text) = GetTaskEventColors(taskItem.Status, taskItem.Color);

            return new CalendarEventViewModel
            {
                Id = $"task-{taskItem.Id}",
                Title = BuildTimedTitle(taskItem.Title, taskItem.StartTime),
                Start = eventStart.ToString(hasStart ? "yyyy-MM-ddTHH:mm:ss" : "yyyy-MM-dd", CultureInfo.InvariantCulture),
                End = eventEnd?.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                AllDay = !hasStart,
                BackgroundColor = background,
                BorderColor = border,
                TextColor = text,
                ClassNames = ["calendar-event", "calendar-event-task", $"calendar-event-task-{taskItem.Status.ToString().ToLowerInvariant()}"],
                ExtendedProps = new CalendarEventExtendedPropsViewModel
                {
                    Type = TaskType,
                    TypeLabel = "Tarefa",
                    TaskItemId = taskItem.Id,
                    Date = start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Status = taskItem.Status.ToString(),
                    StatusLabel = taskItem.Status.ToDisplayName(),
                    Priority = taskItem.Priority.ToString(),
                    PriorityLabel = taskItem.Priority.ToDisplayName(),
                    HabitColor = taskItem.Color,
                    HabitIcon = taskItem.Icon,
                    Time = FormatTaskTimeRange(taskItem.StartTime, taskItem.EndTime),
                    Notes = taskItem.Notes
                }
            };
        }).ToList();
    }

    private async Task<IReadOnlyList<CalendarHabitSource>> GetHabitSourcesAsync(string userId)
    {
        return await _dbContext.Habits
            .AsNoTracking()
            .Where(habit => habit.UserId == userId && habit.Status == ItemStatus.Active)
            .OrderBy(habit => habit.SuggestedTime)
            .ThenBy(habit => habit.Title)
            .Select(habit => new CalendarHabitSource
            {
                Id = habit.Id,
                Title = habit.Title,
                FrequencyType = habit.FrequencyType,
                DaysOfWeek = habit.DaysOfWeek,
                SuggestedTime = habit.SuggestedTime,
                EndTime = habit.EndTime,
                CreatedAt = habit.CreatedAt,
                Color = habit.Color,
                Icon = habit.Icon,
                IdentityName = habit.Identity != null ? habit.Identity.Name : null,
                GoalTitle = habit.Goal != null ? habit.Goal.Title : null,
                LocationName = habit.Location != null ? habit.Location.Name : null
            })
            .ToListAsync();
    }

    private async Task<IReadOnlyList<CalendarSimpleHabitSource>> GetSimpleHabitSourcesAsync(string userId)
    {
        return await _dbContext.SimpleHabits
            .AsNoTracking()
            .Where(simpleHabit => simpleHabit.UserId == userId && simpleHabit.IsActive)
            .OrderBy(simpleHabit => simpleHabit.ScheduledTime)
            .ThenBy(simpleHabit => simpleHabit.Name)
            .Select(simpleHabit => new CalendarSimpleHabitSource
            {
                Id = simpleHabit.Id,
                Name = simpleHabit.Name,
                ScheduledTime = simpleHabit.ScheduledTime,
                CreatedAt = simpleHabit.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<Dictionary<(int HabitId, DateTime Date), CalendarHabitLogSource>> GetLogMapAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var logs = await _dbContext.HabitLogs
            .AsNoTracking()
            .Where(log => log.UserId == userId && log.Date >= startDate && log.Date < endDate)
            .Select(log => new CalendarHabitLogSource
            {
                HabitId = log.HabitId,
                Date = log.Date,
                Status = log.Status,
                Notes = log.Notes
            })
            .ToListAsync();

        return logs.ToDictionary(log => (log.HabitId, log.Date.Date));
    }

    private static bool AppliesOn(CalendarHabitSource habit, DateTime date)
    {
        return HabitScheduleRules.IsDueOnDate(habit.FrequencyType, habit.CreatedAt, habit.DaysOfWeek, date);
    }

    private static bool SimpleHabitAppliesOn(CalendarSimpleHabitSource simpleHabit, DateTime date)
    {
        return date.Date >= simpleHabit.CreatedAt.Date;
    }

    private static int CountPlannedOccurrences(IReadOnlyList<CalendarHabitSource> habits, DateTime start, DateTime end)
    {
        var count = 0;

        foreach (var date in EachDate(start, end))
        {
            count += habits.Count(habit => AppliesOn(habit, date));
        }

        return count;
    }

    private async Task<int> GetCurrentCheckInStreakAsync(string userId)
    {
        var today = AppClock.Today;
        var dates = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .Where(checkIn => checkIn.UserId == userId && checkIn.Date <= today)
            .Select(checkIn => checkIn.Date)
            .ToListAsync();

        var dateSet = dates.Select(date => date.Date).ToHashSet();
        var current = dateSet.Contains(today) ? today : today.AddDays(-1);
        var streak = 0;

        while (dateSet.Contains(current))
        {
            streak++;
            current = current.AddDays(-1);
        }

        return streak;
    }

    private static IEnumerable<DateTime> EachDate(DateTime start, DateTime end)
    {
        for (var date = start.Date; date < end.Date; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    private static bool IncludesType(IReadOnlySet<string> selectedTypes, string type)
    {
        return selectedTypes.Count == 0 || selectedTypes.Contains(type);
    }

    private static string BuildStart(DateTime date, TimeSpan? time)
    {
        return time.HasValue
            ? date.Date.Add(time.Value).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
            : date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string? BuildEnd(DateTime date, TimeSpan? startTime, TimeSpan? endTime)
    {
        if (!startTime.HasValue || !endTime.HasValue)
        {
            return null;
        }

        return date.Date.Add(endTime.Value).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    }

    private static string BuildTimedTitle(string title, TimeSpan? time)
    {
        return time.HasValue ? $"{time.Value:hh\\:mm} {title}" : title;
    }

    private static string? FormatTime(TimeSpan? time)
    {
        return time?.ToString("hh\\:mm", CultureInfo.InvariantCulture);
    }

    private static string? FormatHabitTimeRange(TimeSpan? start, TimeSpan? end)
    {
        if (!start.HasValue)
        {
            return null;
        }

        return end.HasValue
            ? $"{start.Value:hh\\:mm} - {end.Value:hh\\:mm}"
            : $"{start.Value:hh\\:mm}";
    }

    private static (string Background, string Border, string Text) GetImprovementEventColors(HabitLogStatus? status, string habitColor)
    {
        return status switch
        {
            HabitLogStatus.Completed => ("#064e3b", "#34d399", "#d1fae5"),
            HabitLogStatus.Failed => ("#7f1d1d", "#fb7185", "#ffe4e6"),
            HabitLogStatus.Skipped => ("#78350f", "#fbbf24", "#fef3c7"),
            HabitLogStatus.Partial => ("#075985", "#38bdf8", "#e0f2fe"),
            _ => ("#0f172a", string.IsNullOrWhiteSpace(habitColor) ? "#64748b" : habitColor, "#e2e8f0")
        };
    }

    private static (string Background, string Border, string Text) GetTaskEventColors(TaskItemStatus status, string taskColor)
    {
        return status switch
        {
            TaskItemStatus.Completed => ("#064e3b", "#34d399", "#d1fae5"),
            TaskItemStatus.Cancelled => ("#581c87", "#a78bfa", "#ede9fe"),
            TaskItemStatus.Postponed => ("#78350f", "#fbbf24", "#fef3c7"),
            TaskItemStatus.InProgress => ("#0c4a6e", "#38bdf8", "#e0f2fe"),
            _ => ("#1e1b4b", string.IsNullOrWhiteSpace(taskColor) ? "#a78bfa" : taskColor, "#ede9fe")
        };
    }

    private static string GetStatusTone(HabitLogStatus? status)
    {
        return status switch
        {
            HabitLogStatus.Completed => "success",
            HabitLogStatus.Failed => "danger",
            HabitLogStatus.Skipped => "warning",
            HabitLogStatus.Partial => "info",
            _ => "warning"
        };
    }

    private static string GetTaskStatusTone(TaskItemStatus status)
    {
        return status switch
        {
            TaskItemStatus.Completed => "success",
            TaskItemStatus.InProgress => "info",
            TaskItemStatus.Cancelled => "danger",
            TaskItemStatus.Postponed => "neutral",
            _ => "warning"
        };
    }

    private static string? FormatTaskTimeRange(TimeSpan? start, TimeSpan? end)
    {
        if (!start.HasValue)
        {
            return null;
        }

        return end.HasValue
            ? $"{start.Value:hh\\:mm} - {end.Value:hh\\:mm}"
            : $"{start.Value:hh\\:mm}";
    }

    private static string GetMoodFace(MoodLevel mood)
    {
        return mood switch
        {
            MoodLevel.VeryBad => "😞",
            MoodLevel.Bad => "🙁",
            MoodLevel.Neutral => "😐",
            MoodLevel.Good => "🙂",
            MoodLevel.VeryGood => "😄",
            _ => "😐"
        };
    }

    private sealed class CalendarHabitSource
    {
        public int Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public HabitFrequencyType FrequencyType { get; init; }

        public string? DaysOfWeek { get; init; }

        public TimeSpan? SuggestedTime { get; init; }

        public TimeSpan? EndTime { get; init; }

        public DateTime CreatedAt { get; init; }

        public string Color { get; init; } = "#22c55e";

        public string Icon { get; init; } = "repeat-2";

        public string? IdentityName { get; init; }

        public string? GoalTitle { get; init; }

        public string? LocationName { get; init; }
    }

    private sealed class CalendarSimpleHabitSource
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public TimeSpan? ScheduledTime { get; init; }

        public DateTime CreatedAt { get; init; }
    }

    private sealed class CalendarHabitLogSource
    {
        public int HabitId { get; init; }

        public DateTime Date { get; init; }

        public HabitLogStatus Status { get; init; }

        public string? Notes { get; init; }
    }
}

