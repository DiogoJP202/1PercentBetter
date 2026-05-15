using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.ViewModels.Notes;

namespace OnePercentBetter.Web.Services;

public class NoteService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IdentityService _identityService;
    private readonly GoalService _goalService;
    private readonly HabitService _habitService;

    public NoteService(
        ApplicationDbContext dbContext,
        IdentityService identityService,
        GoalService goalService,
        HabitService habitService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _goalService = goalService;
        _habitService = habitService;
    }

    public async Task<IReadOnlyList<NoteListItemViewModel>> GetListAsync(string userId)
    {
        return await _dbContext.Notes
            .AsNoTracking()
            .Where(note => note.UserId == userId)
            .OrderByDescending(note => note.Date)
            .ThenByDescending(note => note.CreatedAt)
            .Select(note => new NoteListItemViewModel
            {
                Id = note.Id,
                Title = note.Title,
                ContentPreview = note.Content.Length > 180 ? note.Content.Substring(0, 180) + "..." : note.Content,
                NoteType = note.NoteType,
                Tags = note.Tags,
                Date = note.Date
            })
            .ToListAsync();
    }

    public async Task<NoteFormViewModel> CreateFormAsync(string userId)
    {
        return await FillOptionsAsync(new NoteFormViewModel(), userId);
    }

    public async Task<NoteFormViewModel?> EditFormAsync(string userId, int id)
    {
        var note = await _dbContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (note is null)
        {
            return null;
        }

        return await FillOptionsAsync(new NoteFormViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            NoteType = note.NoteType,
            Tags = note.Tags,
            GoalId = note.GoalId,
            IdentityId = note.IdentityId,
            HabitId = note.HabitId,
            Date = note.Date
        }, userId);
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, NoteFormViewModel viewModel)
    {
        var errors = new Dictionary<string, string>();

        if (viewModel.IdentityId.HasValue && !await _identityService.ExistsForUserAsync(userId, viewModel.IdentityId.Value))
        {
            errors[nameof(viewModel.IdentityId)] = "Identidade invalida para este usuario.";
        }

        if (viewModel.GoalId.HasValue && !await _goalService.ExistsForUserAsync(userId, viewModel.GoalId.Value))
        {
            errors[nameof(viewModel.GoalId)] = "Objetivo invalido para este usuario.";
        }

        if (viewModel.HabitId.HasValue && !await _habitService.ExistsForUserAsync(userId, viewModel.HabitId.Value))
        {
            errors[nameof(viewModel.HabitId)] = "Habito invalido para este usuario.";
        }

        return errors;
    }

    public async Task<int> CreateAsync(string userId, NoteFormViewModel viewModel)
    {
        var note = new Note
        {
            UserId = userId,
            Title = viewModel.Title.Trim(),
            Content = viewModel.Content.Trim(),
            NoteType = viewModel.NoteType,
            Tags = viewModel.Tags?.Trim(),
            GoalId = viewModel.GoalId,
            IdentityId = viewModel.IdentityId,
            HabitId = viewModel.HabitId,
            Date = viewModel.Date.Date
        };

        _dbContext.Notes.Add(note);
        await _dbContext.SaveChangesAsync();

        return note.Id;
    }

    public async Task<bool> UpdateAsync(string userId, NoteFormViewModel viewModel)
    {
        if (viewModel.Id is null)
        {
            return false;
        }

        var note = await _dbContext.Notes
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == viewModel.Id.Value);

        if (note is null)
        {
            return false;
        }

        note.Title = viewModel.Title.Trim();
        note.Content = viewModel.Content.Trim();
        note.NoteType = viewModel.NoteType;
        note.Tags = viewModel.Tags?.Trim();
        note.GoalId = viewModel.GoalId;
        note.IdentityId = viewModel.IdentityId;
        note.HabitId = viewModel.HabitId;
        note.Date = viewModel.Date.Date;
        note.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<NoteFormViewModel> FillOptionsAsync(NoteFormViewModel viewModel, string userId)
    {
        viewModel.Identities = await _identityService.GetOptionsAsync(userId);
        viewModel.Goals = await _goalService.GetOptionsAsync(userId);
        viewModel.Habits = await _habitService.GetOptionsAsync(userId);
        return viewModel;
    }
}
