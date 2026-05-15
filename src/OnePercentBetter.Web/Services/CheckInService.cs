using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.ViewModels.CheckIns;

namespace OnePercentBetter.Web.Services;

public class CheckInService
{
    private const string MonthPeriod = "month";
    private const string YearPeriod = "year";
    private const string FiveYearsPeriod = "five-years";
    private readonly ApplicationDbContext _dbContext;

    public CheckInService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckInsOverviewViewModel> GetOverviewAsync(string userId, string? period, int? year, int? month)
    {
        var normalizedPeriod = NormalizePeriod(period);
        var today = DateTime.Today;
        var selectedYear = year.GetValueOrDefault(today.Year);
        var selectedMonth = Math.Clamp(month.GetValueOrDefault(today.Month), 1, 12);
        var range = GetRange(normalizedPeriod, selectedYear, selectedMonth);
        var checkIns = await GetCheckInsInRangeAsync(userId, range.Start, range.End);

        return new CheckInsOverviewViewModel
        {
            Period = normalizedPeriod,
            Year = selectedYear,
            Month = selectedMonth,
            PeriodLabel = BuildPeriodLabel(normalizedPeriod, selectedYear, selectedMonth),
            TodayHasCheckIn = await HasCheckInAsync(userId, today),
            Points = BuildPoints(normalizedPeriod, selectedYear, selectedMonth, checkIns),
            Metrics = await BuildMetricsAsync(userId, checkIns),
            SelectedDetail = await GetDetailAsync(userId, today),
            MonthOptions = BuildMonthOptions(selectedMonth),
            YearOptions = await BuildYearOptionsAsync(userId, selectedYear)
        };
    }

    public async Task<IReadOnlyList<CheckInChartPointViewModel>> GetChartPointsAsync(string userId, string? period, int? year, int? month)
    {
        var normalizedPeriod = NormalizePeriod(period);
        var today = DateTime.Today;
        var selectedYear = year.GetValueOrDefault(today.Year);
        var selectedMonth = Math.Clamp(month.GetValueOrDefault(today.Month), 1, 12);
        var range = GetRange(normalizedPeriod, selectedYear, selectedMonth);
        var checkIns = await GetCheckInsInRangeAsync(userId, range.Start, range.End);

        return BuildPoints(normalizedPeriod, selectedYear, selectedMonth, checkIns);
    }

    public async Task<DailyCheckInViewModel> GetTodayAsync(string userId)
    {
        return await GetByDateAsync(userId, DateTime.Today);
    }

    public async Task<DailyCheckInViewModel> GetByDateAsync(string userId, DateTime date)
    {
        var targetDate = date.Date;
        var checkIn = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Date == targetDate);

        if (checkIn is null)
        {
            return new DailyCheckInViewModel { Date = targetDate };
        }

        return new DailyCheckInViewModel
        {
            Date = checkIn.Date,
            Mood = checkIn.Mood,
            EnergyLevel = checkIn.EnergyLevel,
            ProductivityLevel = checkIn.ProductivityLevel,
            DayScore = checkIn.DayScore,
            SmallWin = checkIn.SmallWin,
            MainDifficulty = checkIn.MainDifficulty,
            TomorrowAdjustment = checkIn.TomorrowAdjustment,
            Notes = checkIn.Notes
        };
    }

    public async Task<CheckInDetailViewModel> GetDetailAsync(string userId, DateTime date)
    {
        var targetDate = date.Date;
        var checkIn = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Date == targetDate);

        if (checkIn is null)
        {
            return new CheckInDetailViewModel
            {
                Date = targetDate,
                Exists = false
            };
        }

        return new CheckInDetailViewModel
        {
            Date = checkIn.Date,
            Exists = true,
            Mood = checkIn.Mood,
            EnergyLevel = checkIn.EnergyLevel,
            ProductivityLevel = checkIn.ProductivityLevel,
            DayScore = checkIn.DayScore,
            TotalScore = GetTotalScore(checkIn),
            SmallWin = checkIn.SmallWin,
            MainDifficulty = checkIn.MainDifficulty,
            TomorrowAdjustment = checkIn.TomorrowAdjustment,
            Notes = checkIn.Notes
        };
    }

    public async Task CreateOrUpdateAsync(string userId, DailyCheckInViewModel viewModel)
    {
        var date = viewModel.Date.Date;
        var checkIn = await _dbContext.DailyCheckIns
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Date == date);

        if (checkIn is null)
        {
            checkIn = new DailyCheckIn
            {
                UserId = userId,
                Date = date
            };
            _dbContext.DailyCheckIns.Add(checkIn);
        }

        checkIn.Mood = viewModel.Mood;
        checkIn.EnergyLevel = viewModel.EnergyLevel;
        checkIn.ProductivityLevel = viewModel.ProductivityLevel;
        checkIn.DayScore = viewModel.DayScore;
        checkIn.SmallWin = viewModel.SmallWin?.Trim();
        checkIn.MainDifficulty = viewModel.MainDifficulty?.Trim();
        checkIn.TomorrowAdjustment = viewModel.TomorrowAdjustment?.Trim();
        checkIn.Notes = viewModel.Notes?.Trim();
        checkIn.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    private async Task<IReadOnlyList<DailyCheckIn>> GetCheckInsInRangeAsync(string userId, DateTime start, DateTime end)
    {
        return await _dbContext.DailyCheckIns
            .AsNoTracking()
            .Where(checkIn => checkIn.UserId == userId && checkIn.Date >= start && checkIn.Date <= end)
            .OrderBy(checkIn => checkIn.Date)
            .ToListAsync();
    }

    private async Task<bool> HasCheckInAsync(string userId, DateTime date)
    {
        var targetDate = date.Date;
        return await _dbContext.DailyCheckIns
            .AsNoTracking()
            .AnyAsync(checkIn => checkIn.UserId == userId && checkIn.Date == targetDate);
    }

    private async Task<CheckInMetricsViewModel> BuildMetricsAsync(string userId, IReadOnlyList<DailyCheckIn> checkIns)
    {
        var best = checkIns
            .Select(checkIn => new { checkIn.Date, Score = GetTotalScore(checkIn) })
            .OrderByDescending(item => item.Score)
            .ThenByDescending(item => item.Date)
            .FirstOrDefault();

        return new CheckInMetricsViewModel
        {
            CheckInCount = checkIns.Count,
            AverageScore = checkIns.Count == 0 ? 0 : Math.Round(checkIns.Average(GetTotalScore), 1),
            BestScore = best?.Score,
            BestDate = best?.Date,
            CurrentStreak = await GetCurrentStreakAsync(userId)
        };
    }

    private async Task<int> GetCurrentStreakAsync(string userId)
    {
        var today = DateTime.Today;
        var dates = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .Where(checkIn => checkIn.UserId == userId && checkIn.Date <= today)
            .OrderByDescending(checkIn => checkIn.Date)
            .Select(checkIn => checkIn.Date)
            .ToListAsync();

        if (dates.Count == 0)
        {
            return 0;
        }

        var dateSet = dates.Select(date => date.Date).ToHashSet();
        var cursor = dateSet.Contains(today) ? today : today.AddDays(-1);
        var streak = 0;

        while (dateSet.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    private static IReadOnlyList<CheckInChartPointViewModel> BuildPoints(string period, int year, int month, IReadOnlyList<DailyCheckIn> checkIns)
    {
        return period switch
        {
            YearPeriod => BuildYearPoints(year, checkIns),
            FiveYearsPeriod => BuildFiveYearPoints(year, checkIns),
            _ => BuildMonthPoints(year, month, checkIns)
        };
    }

    private static IReadOnlyList<CheckInChartPointViewModel> BuildMonthPoints(int year, int month, IReadOnlyList<DailyCheckIn> checkIns)
    {
        var byDate = checkIns.ToDictionary(checkIn => checkIn.Date.Date);
        var days = DateTime.DaysInMonth(year, month);
        var points = new List<CheckInChartPointViewModel>();

        for (var day = 1; day <= days; day++)
        {
            var date = new DateTime(year, month, day);
            byDate.TryGetValue(date, out var checkIn);
            var total = checkIn is null ? 0 : GetTotalScore(checkIn);

            points.Add(new CheckInChartPointViewModel
            {
                Key = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Label = day.ToString("00", CultureInfo.InvariantCulture),
                Score = total,
                Count = checkIn is null ? 0 : 1,
                HasCheckIn = checkIn is not null,
                Date = date,
                Summary = checkIn is null ? "Sem check-in registrado." : $"Nota total: {total}/15"
            });
        }

        return points;
    }

    private static IReadOnlyList<CheckInChartPointViewModel> BuildYearPoints(int year, IReadOnlyList<DailyCheckIn> checkIns)
    {
        var byMonth = checkIns.GroupBy(checkIn => checkIn.Date.Month).ToDictionary(group => group.Key, group => group.ToList());
        var points = new List<CheckInChartPointViewModel>();

        for (var month = 1; month <= 12; month++)
        {
            byMonth.TryGetValue(month, out var monthCheckIns);
            var count = monthCheckIns?.Count ?? 0;
            var average = count == 0 ? 0 : Math.Round(monthCheckIns!.Average(GetTotalScore), 1);
            var label = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetAbbreviatedMonthName(month);

            points.Add(new CheckInChartPointViewModel
            {
                Key = $"{year}-{month:00}",
                Label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(label.Replace(".", string.Empty)),
                Score = average,
                Count = count,
                HasCheckIn = count > 0,
                Summary = count == 0 ? "Sem check-ins neste mês." : $"Média mensal: {average}/15 em {count} check-in(s)."
            });
        }

        return points;
    }

    private static IReadOnlyList<CheckInChartPointViewModel> BuildFiveYearPoints(int endYear, IReadOnlyList<DailyCheckIn> checkIns)
    {
        var startYear = endYear - 4;
        var byYear = checkIns.GroupBy(checkIn => checkIn.Date.Year).ToDictionary(group => group.Key, group => group.ToList());
        var points = new List<CheckInChartPointViewModel>();

        for (var year = startYear; year <= endYear; year++)
        {
            byYear.TryGetValue(year, out var yearCheckIns);
            var count = yearCheckIns?.Count ?? 0;
            var average = count == 0 ? 0 : Math.Round(yearCheckIns!.Average(GetTotalScore), 1);

            points.Add(new CheckInChartPointViewModel
            {
                Key = year.ToString(CultureInfo.InvariantCulture),
                Label = year.ToString(CultureInfo.InvariantCulture),
                Score = average,
                Count = count,
                HasCheckIn = count > 0,
                Summary = count == 0 ? "Sem check-ins neste ano." : $"Média anual: {average}/15 em {count} check-in(s)."
            });
        }

        return points;
    }

    private static (DateTime Start, DateTime End) GetRange(string period, int year, int month)
    {
        return period switch
        {
            YearPeriod => (new DateTime(year, 1, 1), new DateTime(year, 12, 31)),
            FiveYearsPeriod => (new DateTime(year - 4, 1, 1), new DateTime(year, 12, 31)),
            _ => (new DateTime(year, month, 1), new DateTime(year, month, DateTime.DaysInMonth(year, month)))
        };
    }

    private static string BuildPeriodLabel(string period, int year, int month)
    {
        return period switch
        {
            YearPeriod => year.ToString(CultureInfo.InvariantCulture),
            FiveYearsPeriod => $"{year - 4} - {year}",
            _ => new DateTime(year, month, 1).ToString("MMMM yyyy", CultureInfo.GetCultureInfo("pt-BR"))
        };
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildYearOptionsAsync(string userId, int selectedYear)
    {
        var years = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .Where(checkIn => checkIn.UserId == userId)
            .Select(checkIn => checkIn.Date.Year)
            .Distinct()
            .ToListAsync();

        years.Add(DateTime.Today.Year);
        years.Add(selectedYear);

        return years
            .Distinct()
            .OrderByDescending(year => year)
            .Select(year => new SelectListItem
            {
                Value = year.ToString(CultureInfo.InvariantCulture),
                Text = year.ToString(CultureInfo.InvariantCulture),
                Selected = year == selectedYear
            })
            .ToList();
    }

    private static IReadOnlyList<SelectListItem> BuildMonthOptions(int selectedMonth)
    {
        var culture = CultureInfo.GetCultureInfo("pt-BR");

        return Enumerable.Range(1, 12)
            .Select(month => new SelectListItem
            {
                Value = month.ToString(CultureInfo.InvariantCulture),
                Text = culture.DateTimeFormat.GetMonthName(month),
                Selected = month == selectedMonth
            })
            .ToList();
    }

    private static string NormalizePeriod(string? period)
    {
        return period is YearPeriod or FiveYearsPeriod ? period : MonthPeriod;
    }

    private static int GetTotalScore(DailyCheckIn checkIn)
    {
        return checkIn.DayScore + checkIn.EnergyLevel + checkIn.ProductivityLevel;
    }
}
