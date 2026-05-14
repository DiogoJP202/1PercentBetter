using Microsoft.EntityFrameworkCore;
using OnePercentBetter.Web.Data;
using OnePercentBetter.Web.ViewModels.Shared;

namespace OnePercentBetter.Web.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SelectOptionViewModel>> GetOptionsAsync(string userId)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.UserId == null || category.UserId == userId)
            .OrderBy(category => category.Name)
            .Select(category => new SelectOptionViewModel
            {
                Value = category.Id.ToString(),
                Text = category.Name
            })
            .ToListAsync();
    }
}
