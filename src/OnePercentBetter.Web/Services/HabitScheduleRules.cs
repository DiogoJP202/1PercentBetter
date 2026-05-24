using OnePercentBetter.Web.Models.Enums;

namespace OnePercentBetter.Web.Services;

public static class HabitScheduleRules
{
    public static bool IsDueOnDate(
        HabitFrequencyType frequencyType,
        DateTime createdAt,
        string? daysOfWeek,
        DateTime date)
    {
        var targetDate = date.Date;
        if (targetDate < createdAt.Date)
        {
            return false;
        }

        var configuredDays = ParseDaysOfWeek(daysOfWeek);
        return frequencyType switch
        {
            HabitFrequencyType.Daily => true,
            HabitFrequencyType.SpecificDays => configuredDays.Contains(targetDate.DayOfWeek),
            HabitFrequencyType.Weekly => configuredDays.Count > 0
                ? configuredDays.Contains(targetDate.DayOfWeek)
                : targetDate.DayOfWeek == createdAt.DayOfWeek,
            HabitFrequencyType.Monthly => targetDate.Day == Math.Min(createdAt.Day, DateTime.DaysInMonth(targetDate.Year, targetDate.Month)),
            _ => false
        };
    }

    public static IReadOnlySet<DayOfWeek> ParseDaysOfWeek(string? daysOfWeek)
    {
        if (string.IsNullOrWhiteSpace(daysOfWeek))
        {
            return new HashSet<DayOfWeek>();
        }

        return daysOfWeek
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(day => Enum.TryParse<DayOfWeek>(day, true, out var parsed) ? parsed : (DayOfWeek?)null)
            .Where(day => day.HasValue)
            .Select(day => day!.Value)
            .ToHashSet();
    }

    public static DateTime? GetNextDueDate(
        HabitFrequencyType frequencyType,
        DateTime createdAt,
        string? daysOfWeek,
        DateTime referenceDate,
        int lookAheadDays = 366)
    {
        var startDate = referenceDate.Date;
        for (var offset = 0; offset <= lookAheadDays; offset++)
        {
            var candidate = startDate.AddDays(offset);
            if (IsDueOnDate(frequencyType, createdAt, daysOfWeek, candidate))
            {
                return candidate;
            }
        }

        return null;
    }
}
