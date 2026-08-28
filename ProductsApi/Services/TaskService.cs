using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Models;

namespace ProductsApi.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    // Whitelist of allowed sort fields. Unknown sortBy values fall back to the default.
    private const string DefaultSortField = "createdat";

    public async Task<PagedResult<TaskItem>> GetAllAsync(TaskFilterParams filterParams)
    {
        var query = _context.TaskItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filterParams.Search))
        {
            var search = filterParams.Search.ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(search));
        }

        if (filterParams.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == filterParams.IsCompleted.Value);
        }

        var sortField = filterParams.SortBy?.ToLowerInvariant() ?? DefaultSortField;

        query = sortField switch
        {
            "title" => query.OrderBy(t => t.Title),
            "duedate" => query.OrderBy(t => t.DueDate),
            "iscompleted" => query.OrderBy(t => t.IsCompleted),
            _ => query.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var page = filterParams.Page < 1 ? 1 : filterParams.Page;
        var pageSize = filterParams.PageSize < 1 ? 1 : filterParams.PageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<TaskItem>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(int id, TaskItem task)
    {
        var existing = await _context.TaskItems.FindAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Title = task.Title;
        existing.IsCompleted = task.IsCompleted;
        existing.DueDate = task.DueDate;
        existing.UserId = task.UserId;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.TaskItems.FindAsync(id);
        if (existing is null)
        {
            return false;
        }

        _context.TaskItems.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
