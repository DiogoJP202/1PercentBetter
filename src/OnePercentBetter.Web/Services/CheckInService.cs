using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.ViewModels.CheckIns;

namespace OnePercentBetter.Web.Services;

public class CheckInService
{
    private readonly ApplicationDbContext _dbContext;

    public CheckInService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DailyCheckInViewModel> GetTodayAsync(string userId)
    {
        var today = DateTime.Today;
        var checkIn = await _dbContext.DailyCheckIns
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Date == today);

        if (checkIn is null)
        {
            return new DailyCheckInViewModel { Date = today };
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
}
