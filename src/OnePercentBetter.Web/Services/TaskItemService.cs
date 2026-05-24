using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.Models.Entities;
using OnePercentBetter.Web.Models.Enums;
using OnePercentBetter.Web.ViewModels.Shared;
using OnePercentBetter.Web.ViewModels.Tasks;
using System.Text.RegularExpressions;

namespace OnePercentBetter.Web.Services;

public class TaskItemService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CategoryService _categoryService;
    private readonly IdentityService _identityService;
    private readonly GoalService _goalService;
    private readonly HabitService _habitService;

    public TaskItemService(
        ApplicationDbContext dbContext,
        CategoryService categoryService,
        IdentityService identityService,
        GoalService goalService,
        HabitService habitService)
    {
        _dbContext = dbContext;
        _categoryService = categoryService;
        _identityService = identityService;
        _goalService = goalService;
        _habitService = habitService;
    }

    public async Task<TaskItemListViewModel> GetListAsync(string userId, TaskFiltersViewModel? filters)
    {
        var normalizedFilters = await FillFilterOptionsAsync(filters ?? new TaskFiltersViewModel(), userId);
        var today = DateTime.Today;

        var query = _dbContext.TaskItems
            .AsNoTracking()
            .Include(taskItem => taskItem.Category)
            .Include(taskItem => taskItem.Identity)
            .Include(taskItem => taskItem.Goal)
            .Include(taskItem => taskItem.Habit)
            .Include(taskItem => taskItem.TaskItemTags)
                .ThenInclude(taskItemTag => taskItemTag.TaskTag)
            .Where(taskItem => taskItem.UserId == userId);

        query = ApplyFilters(query, normalizedFilters, today);

        var entities = await query
            .OrderBy(taskItem => taskItem.TaskDate.HasValue ? 0 : 1)
            .ThenBy(taskItem => taskItem.TaskDate)
            .ThenBy(taskItem => taskItem.StartTime.HasValue ? 0 : 1)
            .ThenBy(taskItem => taskItem.StartTime)
            .ThenByDescending(taskItem => taskItem.Priority)
            .ThenByDescending(taskItem => taskItem.CreatedAt)
            .ToListAsync();

        var items = entities
            .Select(taskItem => ToCardViewModel(taskItem, today))
            .ToList();

        var summaryBase = _dbContext.TaskItems
            .AsNoTracking()
            .Where(taskItem => taskItem.UserId == userId);

        var pendingCount = await summaryBase.CountAsync(taskItem =>
            taskItem.Status == TaskItemStatus.Pending
            || taskItem.Status == TaskItemStatus.InProgress
            || taskItem.Status == TaskItemStatus.Postponed);
        var todayCount = await summaryBase.CountAsync(taskItem =>
            (taskItem.Status == TaskItemStatus.Pending
             || taskItem.Status == TaskItemStatus.InProgress
             || taskItem.Status == TaskItemStatus.Postponed)
            && taskItem.TaskDate.HasValue
            && taskItem.TaskDate.Value == today);
        var overdueCount = await summaryBase.CountAsync(taskItem =>
            (taskItem.Status == TaskItemStatus.Pending
             || taskItem.Status == TaskItemStatus.InProgress
             || taskItem.Status == TaskItemStatus.Postponed)
            && ((taskItem.DueDate.HasValue && taskItem.DueDate.Value < today)
                || (taskItem.TaskDate.HasValue && taskItem.TaskDate.Value < today)));
        var completedCount = await summaryBase.CountAsync(taskItem => taskItem.Status == TaskItemStatus.Completed);

        return new TaskItemListViewModel
        {
            Filters = normalizedFilters,
            Items = items,
            TodayItems = items.Where(taskItem => taskItem.TaskDate.HasValue && taskItem.TaskDate.Value == today && IsOpenStatus(taskItem.Status)).ToList(),
            FutureItems = items.Where(taskItem => taskItem.TaskDate.HasValue && taskItem.TaskDate.Value > today && IsOpenStatus(taskItem.Status)).ToList(),
            OverdueItems = items.Where(taskItem => taskItem.IsOverdue && IsOpenStatus(taskItem.Status)).ToList(),
            CompletedItems = items.Where(taskItem => taskItem.Status == TaskItemStatus.Completed).ToList(),
            TodayCount = todayCount,
            PendingCount = pendingCount,
            OverdueCount = overdueCount,
            CompletedCount = completedCount
        };
    }

    public async Task<TaskItemFormViewModel> CreateFormAsync(string userId)
    {
        return await FillOptionsAsync(new TaskItemFormViewModel(), userId);
    }

    public async Task<TaskItemFormViewModel?> EditFormAsync(string userId, int id)
    {
        var taskItem = await _dbContext.TaskItems
            .AsNoTracking()
            .Include(item => item.TaskItemTags)
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (taskItem is null)
        {
            return null;
        }

        return await FillOptionsAsync(new TaskItemFormViewModel
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Notes = taskItem.Notes,
            Status = taskItem.Status,
            Priority = taskItem.Priority,
            TaskDate = taskItem.TaskDate,
            StartTime = taskItem.StartTime,
            EndTime = taskItem.EndTime,
            DueDate = taskItem.DueDate,
            CategoryId = taskItem.CategoryId,
            IdentityId = taskItem.IdentityId,
            GoalId = taskItem.GoalId,
            HabitId = taskItem.HabitId,
            Color = taskItem.Color,
            Icon = taskItem.Icon,
            ShowOnCalendar = taskItem.ShowOnCalendar,
            SelectedTagIds = taskItem.TaskItemTags.Select(taskItemTag => taskItemTag.TaskTagId).ToList()
        }, userId);
    }

    public async Task PopulateOptionsAsync(TaskItemFormViewModel viewModel, string userId)
    {
        await FillOptionsAsync(viewModel, userId);
    }

    public async Task<(bool Success, string? Error, TaskTagBadgeViewModel? Tag)> SaveTagAsync(string userId, TaskTagEditViewModel viewModel)
    {
        var name = viewModel.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Informe o nome da tag.", null);
        }

        if (name.Length > 64)
        {
            return (false, "Use no máximo 64 caracteres no nome da tag.", null);
        }

        var normalizedName = NormalizeTagName(name);
        var color = NormalizeTagColor(viewModel.Color);

        if (viewModel.Id.HasValue && viewModel.Id.Value > 0)
        {
            var existing = await _dbContext.TaskTags
                .FirstOrDefaultAsync(taskTag => taskTag.UserId == userId && taskTag.Id == viewModel.Id.Value);

            if (existing is null)
            {
                return (false, "Tag não encontrada para este usuário.", null);
            }

            var conflict = await _dbContext.TaskTags
                .AsNoTracking()
                .AnyAsync(taskTag =>
                    taskTag.UserId == userId
                    && taskTag.Id != existing.Id
                    && taskTag.Name.ToLower() == normalizedName);

            if (conflict)
            {
                return (false, "Já existe uma tag com esse nome.", null);
            }

            existing.Name = name;
            existing.Color = color;
            await _dbContext.SaveChangesAsync();

            return (true, null, new TaskTagBadgeViewModel
            {
                Id = existing.Id,
                Name = existing.Name,
                Color = existing.Color
            });
        }

        var existingByName = await _dbContext.TaskTags
            .FirstOrDefaultAsync(taskTag => taskTag.UserId == userId && taskTag.Name.ToLower() == normalizedName);

        if (existingByName is not null)
        {
            existingByName.Color = color;
            await _dbContext.SaveChangesAsync();

            return (true, null, new TaskTagBadgeViewModel
            {
                Id = existingByName.Id,
                Name = existingByName.Name,
                Color = existingByName.Color
            });
        }

        var taskTag = new TaskTag
        {
            UserId = userId,
            Name = name,
            Color = color
        };

        _dbContext.TaskTags.Add(taskTag);
        await _dbContext.SaveChangesAsync();

        return (true, null, new TaskTagBadgeViewModel
        {
            Id = taskTag.Id,
            Name = taskTag.Name,
            Color = taskTag.Color
        });
    }

    public async Task<(bool Success, string? Error)> DeleteTagAsync(string userId, int tagId)
    {
        var taskTag = await _dbContext.TaskTags
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == tagId);

        if (taskTag is null)
        {
            return (false, "Tag não encontrada para este usuário.");
        }

        _dbContext.TaskTags.Remove(taskTag);
        await _dbContext.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyDictionary<string, string>> ValidateFormAsync(string userId, TaskItemFormViewModel viewModel)
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

        if (viewModel.GoalId.HasValue && !await _goalService.ExistsForUserAsync(userId, viewModel.GoalId.Value))
        {
            errors[nameof(viewModel.GoalId)] = "Objetivo inválido para este usuário.";
        }

        if (viewModel.HabitId.HasValue && !await _habitService.ExistsForUserAsync(userId, viewModel.HabitId.Value))
        {
            errors[nameof(viewModel.HabitId)] = "Hábito inválido para este usuário.";
        }

        if (viewModel.StartTime.HasValue && viewModel.EndTime.HasValue && viewModel.EndTime <= viewModel.StartTime)
        {
            errors[nameof(viewModel.EndTime)] = "O horário final deve ser maior que o horário inicial.";
        }

        if (viewModel.TaskDate.HasValue && viewModel.DueDate.HasValue && viewModel.DueDate.Value.Date < viewModel.TaskDate.Value.Date)
        {
            errors[nameof(viewModel.DueDate)] = "O prazo não pode ser anterior à data da tarefa.";
        }

        if (!TaskVisualOptions.IsAllowedIcon(viewModel.Icon))
        {
            errors[nameof(viewModel.Icon)] = "Escolha um ícone válido para a tarefa.";
        }

        if (viewModel.SelectedTagIds.Count > 0)
        {
            var distinctIds = viewModel.SelectedTagIds.Distinct().ToList();
            var validCount = await _dbContext.TaskTags
                .AsNoTracking()
                .CountAsync(taskTag => taskTag.UserId == userId && distinctIds.Contains(taskTag.Id));

            if (validCount != distinctIds.Count)
            {
                errors[nameof(viewModel.SelectedTagIds)] = "Uma ou mais tags selecionadas são inválidas para este usuário.";
            }
        }

        if (ParseNewTags(viewModel.NewTags).Count > 12)
        {
            errors[nameof(viewModel.NewTags)] = "Use no máximo 12 tags novas por vez.";
        }

        return errors;
    }

    public async Task<int> CreateAsync(string userId, TaskItemFormViewModel viewModel)
    {
        var taskItem = new TaskItem
        {
            UserId = userId,
            Title = viewModel.Title.Trim(),
            Description = viewModel.Description?.Trim(),
            Notes = viewModel.Notes?.Trim(),
            Status = viewModel.Status,
            Priority = viewModel.Priority,
            TaskDate = viewModel.TaskDate?.Date,
            StartTime = viewModel.StartTime,
            EndTime = viewModel.EndTime,
            DueDate = viewModel.DueDate?.Date,
            CategoryId = viewModel.CategoryId,
            IdentityId = viewModel.IdentityId,
            GoalId = viewModel.GoalId,
            HabitId = viewModel.HabitId,
            Color = NormalizeColor(viewModel.Color),
            Icon = NormalizeIcon(viewModel.Icon),
            ShowOnCalendar = viewModel.ShowOnCalendar,
            CompletedAt = viewModel.Status == TaskItemStatus.Completed ? DateTime.UtcNow : null
        };

        var tags = await ResolveTagsAsync(userId, viewModel.SelectedTagIds, viewModel.NewTags);
        foreach (var tag in tags)
        {
            taskItem.TaskItemTags.Add(new TaskItemTag { TaskTag = tag });
        }

        _dbContext.TaskItems.Add(taskItem);
        await _dbContext.SaveChangesAsync();

        return taskItem.Id;
    }

    public async Task<bool> UpdateAsync(string userId, TaskItemFormViewModel viewModel)
    {
        if (!viewModel.Id.HasValue)
        {
            return false;
        }

        var taskItem = await _dbContext.TaskItems
            .Include(item => item.TaskItemTags)
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == viewModel.Id.Value);

        if (taskItem is null)
        {
            return false;
        }

        var statusChangedToCompleted = taskItem.Status != TaskItemStatus.Completed && viewModel.Status == TaskItemStatus.Completed;
        var statusChangedFromCompleted = taskItem.Status == TaskItemStatus.Completed && viewModel.Status != TaskItemStatus.Completed;

        taskItem.Title = viewModel.Title.Trim();
        taskItem.Description = viewModel.Description?.Trim();
        taskItem.Notes = viewModel.Notes?.Trim();
        taskItem.Status = viewModel.Status;
        taskItem.Priority = viewModel.Priority;
        taskItem.TaskDate = viewModel.TaskDate?.Date;
        taskItem.StartTime = viewModel.StartTime;
        taskItem.EndTime = viewModel.EndTime;
        taskItem.DueDate = viewModel.DueDate?.Date;
        taskItem.CategoryId = viewModel.CategoryId;
        taskItem.IdentityId = viewModel.IdentityId;
        taskItem.GoalId = viewModel.GoalId;
        taskItem.HabitId = viewModel.HabitId;
        taskItem.Color = NormalizeColor(viewModel.Color);
        taskItem.Icon = NormalizeIcon(viewModel.Icon);
        taskItem.ShowOnCalendar = viewModel.ShowOnCalendar;
        taskItem.UpdatedAt = DateTime.UtcNow;

        if (statusChangedToCompleted)
        {
            taskItem.CompletedAt = DateTime.UtcNow;
        }
        else if (statusChangedFromCompleted)
        {
            taskItem.CompletedAt = null;
        }

        taskItem.TaskItemTags.Clear();
        var tags = await ResolveTagsAsync(userId, viewModel.SelectedTagIds, viewModel.NewTags);
        foreach (var tag in tags)
        {
            taskItem.TaskItemTags.Add(new TaskItemTag { TaskTagId = tag.Id });
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string userId, int id)
    {
        var taskItem = await _dbContext.TaskItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (taskItem is null || taskItem.IsDeleted)
        {
            return false;
        }

        taskItem.IsDeleted = true;
        taskItem.DeletedAt = DateTime.UtcNow;
        taskItem.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(string userId, int id, TaskItemStatus status)
    {
        var taskItem = await _dbContext.TaskItems
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (taskItem is null)
        {
            return false;
        }

        taskItem.Status = status;
        taskItem.UpdatedAt = DateTime.UtcNow;
        taskItem.CompletedAt = status == TaskItemStatus.Completed ? DateTime.UtcNow : null;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PostponeAsync(string userId, int id, int days = 1)
    {
        var taskItem = await _dbContext.TaskItems
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);

        if (taskItem is null)
        {
            return false;
        }

        var postponeDays = Math.Max(1, days);
        var baseDate = taskItem.TaskDate?.Date ?? DateTime.Today;
        taskItem.TaskDate = baseDate.AddDays(postponeDays);

        if (taskItem.DueDate.HasValue && taskItem.DueDate.Value.Date < taskItem.TaskDate.Value.Date)
        {
            taskItem.DueDate = taskItem.TaskDate.Value.Date;
        }

        taskItem.Status = TaskItemStatus.Postponed;
        taskItem.CompletedAt = null;
        taskItem.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId, bool onlyOpen = false)
    {
        var query = _dbContext.TaskItems
            .AsNoTracking()
            .Where(taskItem => taskItem.UserId == userId);

        if (onlyOpen)
        {
            query = query.Where(taskItem =>
                taskItem.Status == TaskItemStatus.Pending
                || taskItem.Status == TaskItemStatus.InProgress
                || taskItem.Status == TaskItemStatus.Postponed);
        }

        return await query
            .OrderBy(taskItem => taskItem.TaskDate.HasValue ? 0 : 1)
            .ThenBy(taskItem => taskItem.TaskDate)
            .ThenBy(taskItem => taskItem.Title)
            .Select(taskItem => new SelectOptionViewModel
            {
                Value = taskItem.Id.ToString(),
                Text = taskItem.Title
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TaskItemCardViewModel>> GetTodayTasksAsync(string userId)
    {
        var today = DateTime.Today;
        var taskItems = await _dbContext.TaskItems
            .AsNoTracking()
            .Include(taskItem => taskItem.Category)
            .Include(taskItem => taskItem.Identity)
            .Include(taskItem => taskItem.Goal)
            .Include(taskItem => taskItem.Habit)
            .Include(taskItem => taskItem.TaskItemTags)
                .ThenInclude(taskItemTag => taskItemTag.TaskTag)
            .Where(taskItem => taskItem.UserId == userId && taskItem.TaskDate.HasValue && taskItem.TaskDate.Value == today)
            .OrderBy(taskItem => taskItem.StartTime)
            .ThenByDescending(taskItem => taskItem.Priority)
            .ToListAsync();

        return taskItems
            .Select(taskItem => ToCardViewModel(taskItem, today))
            .ToList();
    }

    public async Task<IReadOnlyList<TaskItemCardViewModel>> GetTasksByGoalAsync(string userId, int goalId)
    {
        var today = DateTime.Today;
        var taskItems = await _dbContext.TaskItems
            .AsNoTracking()
            .Include(taskItem => taskItem.Category)
            .Include(taskItem => taskItem.Identity)
            .Include(taskItem => taskItem.Goal)
            .Include(taskItem => taskItem.Habit)
            .Include(taskItem => taskItem.TaskItemTags)
                .ThenInclude(taskItemTag => taskItemTag.TaskTag)
            .Where(taskItem => taskItem.UserId == userId && taskItem.GoalId == goalId)
            .OrderBy(taskItem => taskItem.Status == TaskItemStatus.Completed ? 1 : 0)
            .ThenBy(taskItem => taskItem.TaskDate)
            .ThenBy(taskItem => taskItem.Title)
            .ToListAsync();

        return taskItems
            .Select(taskItem => ToCardViewModel(taskItem, today))
            .ToList();
    }

    public async Task<IReadOnlyList<TaskItemCardViewModel>> GetTasksByIdentityAsync(string userId, int identityId)
    {
        var today = DateTime.Today;
        var taskItems = await _dbContext.TaskItems
            .AsNoTracking()
            .Include(taskItem => taskItem.Category)
            .Include(taskItem => taskItem.Identity)
            .Include(taskItem => taskItem.Goal)
            .Include(taskItem => taskItem.Habit)
            .Include(taskItem => taskItem.TaskItemTags)
                .ThenInclude(taskItemTag => taskItemTag.TaskTag)
            .Where(taskItem => taskItem.UserId == userId && taskItem.IdentityId == identityId)
            .OrderBy(taskItem => taskItem.Status == TaskItemStatus.Completed ? 1 : 0)
            .ThenBy(taskItem => taskItem.TaskDate)
            .ThenBy(taskItem => taskItem.Title)
            .ToListAsync();

        return taskItems
            .Select(taskItem => ToCardViewModel(taskItem, today))
            .ToList();
    }

    public async Task<TaskItemDetailsViewModel?> GetDetailsAsync(string userId, int id)
    {
        var today = DateTime.Today;
        var task = await _dbContext.TaskItems
            .AsNoTracking()
            .Include(taskItem => taskItem.Category)
            .Include(taskItem => taskItem.Identity)
            .Include(taskItem => taskItem.Goal)
            .Include(taskItem => taskItem.Habit)
            .Include(taskItem => taskItem.TaskItemTags)
                .ThenInclude(taskItemTag => taskItemTag.TaskTag)
            .Where(taskItem => taskItem.UserId == userId && taskItem.Id == id)
            .Select(taskItem => taskItem)
            .FirstOrDefaultAsync();

        if (task is null)
        {
            return null;
        }

        var relatedNotes = await _dbContext.Notes
            .AsNoTracking()
            .Where(note => note.UserId == userId && note.TaskItemId == id)
            .OrderByDescending(note => note.Date)
            .ThenByDescending(note => note.CreatedAt)
            .Select(note => note.Title)
            .Take(5)
            .ToListAsync();

        return new TaskItemDetailsViewModel
        {
            Task = ToCardViewModel(task, today),
            RelatedNotes = relatedNotes
        };
    }

    public IQueryable<TaskItem> QueryUserTasks(string userId)
    {
        return _dbContext.TaskItems
            .AsNoTracking()
            .Where(taskItem => taskItem.UserId == userId);
    }

    private async Task<TaskFiltersViewModel> FillFilterOptionsAsync(TaskFiltersViewModel filters, string userId)
    {
        filters.Tags = await GetTagOptionsAsync(userId);
        filters.Goals = await _goalService.GetOptionsAsync(userId);
        filters.Identities = await _identityService.GetOptionsAsync(userId);
        filters.Categories = await _categoryService.GetOptionsAsync(userId);
        filters.View = NormalizeView(filters.View);
        filters.GroupBy = NormalizeGroupBy(filters.GroupBy);
        return filters;
    }

    private async Task<TaskItemFormViewModel> FillOptionsAsync(TaskItemFormViewModel viewModel, string userId)
    {
        viewModel.Categories = await _categoryService.GetOptionsAsync(userId);
        viewModel.Identities = await _identityService.GetOptionsAsync(userId);
        viewModel.Goals = await _goalService.GetOptionsAsync(userId);
        viewModel.Habits = await _habitService.GetOptionsAsync(userId);
        viewModel.Tags = await GetTagOptionsAsync(userId);
        viewModel.TagItems = await GetTagItemsAsync(userId);
        viewModel.Color = NormalizeColor(viewModel.Color);
        viewModel.Icon = NormalizeIcon(viewModel.Icon);
        return viewModel;
    }

    private async Task<IReadOnlyList<SelectOptionViewModel>> GetTagOptionsAsync(string userId)
    {
        return await _dbContext.TaskTags
            .AsNoTracking()
            .Where(taskTag => taskTag.UserId == userId)
            .OrderBy(taskTag => taskTag.Name)
            .Select(taskTag => new SelectOptionViewModel
            {
                Value = taskTag.Id.ToString(),
                Text = taskTag.Name
            })
            .ToListAsync();
    }

    private async Task<IReadOnlyList<TaskTagBadgeViewModel>> GetTagItemsAsync(string userId)
    {
        return await _dbContext.TaskTags
            .AsNoTracking()
            .Where(taskTag => taskTag.UserId == userId)
            .OrderBy(taskTag => taskTag.Name)
            .Select(taskTag => new TaskTagBadgeViewModel
            {
                Id = taskTag.Id,
                Name = taskTag.Name,
                Color = taskTag.Color
            })
            .ToListAsync();
    }

    private IQueryable<TaskItem> ApplyFilters(IQueryable<TaskItem> query, TaskFiltersViewModel filters, DateTime today)
    {
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim();
            query = query.Where(taskItem =>
                taskItem.Title.Contains(search)
                || (taskItem.Description != null && taskItem.Description.Contains(search))
                || (taskItem.Notes != null && taskItem.Notes.Contains(search))
                || (taskItem.Goal != null && taskItem.Goal.Title.Contains(search))
                || taskItem.TaskItemTags.Any(taskItemTag => taskItemTag.TaskTag != null && taskItemTag.TaskTag.Name.Contains(search)));
        }

        if (filters.Priority.HasValue)
        {
            query = query.Where(taskItem => taskItem.Priority == filters.Priority.Value);
        }

        if (filters.GoalId.HasValue)
        {
            query = query.Where(taskItem => taskItem.GoalId == filters.GoalId.Value);
        }

        if (filters.IdentityId.HasValue)
        {
            query = query.Where(taskItem => taskItem.IdentityId == filters.IdentityId.Value);
        }

        if (filters.CategoryId.HasValue)
        {
            query = query.Where(taskItem => taskItem.CategoryId == filters.CategoryId.Value);
        }

        if (filters.TagId.HasValue)
        {
            query = query.Where(taskItem => taskItem.TaskItemTags.Any(taskItemTag => taskItemTag.TaskTagId == filters.TagId.Value));
        }

        if (filters.WithTime.HasValue)
        {
            query = filters.WithTime.Value
                ? query.Where(taskItem => taskItem.StartTime.HasValue)
                : query.Where(taskItem => !taskItem.StartTime.HasValue);
        }

        query = filters.View switch
        {
            "today" => query.Where(taskItem => taskItem.TaskDate.HasValue && taskItem.TaskDate.Value == today),
            "week" => query.Where(taskItem =>
                taskItem.TaskDate.HasValue
                && taskItem.TaskDate.Value >= today
                && taskItem.TaskDate.Value <= today.AddDays(6)),
            "overdue" => query.Where(taskItem =>
                (taskItem.Status == TaskItemStatus.Pending
                 || taskItem.Status == TaskItemStatus.InProgress
                 || taskItem.Status == TaskItemStatus.Postponed)
                && ((taskItem.DueDate.HasValue && taskItem.DueDate.Value < today)
                    || (taskItem.TaskDate.HasValue && taskItem.TaskDate.Value < today))),
            "completed" => query.Where(taskItem => taskItem.Status == TaskItemStatus.Completed),
            _ => query
        };

        return query;
    }

    private async Task<IReadOnlyList<TaskTag>> ResolveTagsAsync(string userId, IReadOnlyList<int> selectedTagIds, string? newTagsRaw)
    {
        var selectedIds = selectedTagIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var selectedTags = selectedIds.Count == 0
            ? []
            : await _dbContext.TaskTags
                .Where(taskTag => taskTag.UserId == userId && selectedIds.Contains(taskTag.Id))
                .ToListAsync();

        var newNames = ParseNewTags(newTagsRaw);
        if (newNames.Count == 0)
        {
            return selectedTags;
        }

        var normalizedNameSet = newNames.Select(NormalizeTagName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingNamedTags = await _dbContext.TaskTags
            .Where(taskTag => taskTag.UserId == userId && normalizedNameSet.Contains(taskTag.Name.ToLower()))
            .ToListAsync();

        var existingMap = existingNamedTags.ToDictionary(tag => tag.Name.ToLower(), StringComparer.OrdinalIgnoreCase);

        foreach (var name in newNames)
        {
            var normalized = NormalizeTagName(name);
            if (existingMap.TryGetValue(normalized, out var existing))
            {
                selectedTags = selectedTags.Append(existing).ToList();
                continue;
            }

            var newTag = new TaskTag
            {
                UserId = userId,
                Name = name,
                Color = TaskVisualOptions.DefaultColor
            };
            _dbContext.TaskTags.Add(newTag);
            selectedTags = selectedTags.Append(newTag).ToList();
            existingMap[normalized] = newTag;
        }

        return selectedTags
            .GroupBy(tag => tag.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    private static List<string> ParseNewTags(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return [];
        }

        return raw
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(12)
            .ToList();
    }

    private static string NormalizeView(string? view)
    {
        if (string.IsNullOrWhiteSpace(view))
        {
            return "all";
        }

        var normalized = view.Trim().ToLowerInvariant();
        return normalized is "all" or "today" or "week" or "overdue" or "completed"
            ? normalized
            : "all";
    }

    private static string NormalizeGroupBy(string? groupBy)
    {
        if (string.IsNullOrWhiteSpace(groupBy))
        {
            return "date";
        }

        var normalized = groupBy.Trim().ToLowerInvariant();
        return normalized is "date" or "goal" or "priority" ? normalized : "date";
    }

    private static string NormalizeColor(string? color)
    {
        return string.IsNullOrWhiteSpace(color) ? TaskVisualOptions.DefaultColor : color.Trim();
    }

    private static string NormalizeIcon(string? icon)
    {
        if (!TaskVisualOptions.IsAllowedIcon(icon))
        {
            return TaskVisualOptions.DefaultIcon;
        }

        return icon!.Trim();
    }

    private static string NormalizeTagName(string name)
    {
        return name.Trim().ToLowerInvariant();
    }

    private static string NormalizeTagColor(string? color)
    {
        var candidate = string.IsNullOrWhiteSpace(color) ? TaskVisualOptions.DefaultColor : color.Trim();
        return Regex.IsMatch(candidate, "^#[0-9a-fA-F]{6}$") ? candidate : TaskVisualOptions.DefaultColor;
    }

    private static bool IsOverdue(DateTime? taskDate, DateTime? dueDate, DateTime today)
    {
        return (dueDate.HasValue && dueDate.Value.Date < today)
            || (taskDate.HasValue && taskDate.Value.Date < today);
    }

    private static bool IsOpenStatus(TaskItemStatus status)
    {
        return status is TaskItemStatus.Pending or TaskItemStatus.InProgress or TaskItemStatus.Postponed;
    }

    private static TaskItemCardViewModel ToCardViewModel(TaskItem taskItem, DateTime today)
    {
        return new TaskItemCardViewModel
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status,
            Priority = taskItem.Priority,
            TaskDate = taskItem.TaskDate,
            StartTime = taskItem.StartTime,
            EndTime = taskItem.EndTime,
            DueDate = taskItem.DueDate,
            CategoryName = taskItem.Category != null ? taskItem.Category.Name : null,
            IdentityName = taskItem.Identity != null ? taskItem.Identity.Name : null,
            GoalTitle = taskItem.Goal != null ? taskItem.Goal.Title : null,
            HabitTitle = taskItem.Habit != null ? taskItem.Habit.Title : null,
            Color = taskItem.Color,
            Icon = taskItem.Icon,
            ShowOnCalendar = taskItem.ShowOnCalendar,
            CompletedAt = taskItem.CompletedAt,
            IsOverdue = IsOpenStatus(taskItem.Status) && IsOverdue(taskItem.TaskDate, taskItem.DueDate, today),
            Tags = taskItem.TaskItemTags
                .Where(taskItemTag => taskItemTag.TaskTag != null)
                .Select(taskItemTag => new TaskTagBadgeViewModel
                {
                    Id = taskItemTag.TaskTagId,
                    Name = taskItemTag.TaskTag!.Name,
                    Color = taskItemTag.TaskTag.Color
                })
                .OrderBy(tag => tag.Name)
                .ToList()
        };
    }
}
