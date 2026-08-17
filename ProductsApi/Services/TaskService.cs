using ProductsApi.Models;

namespace ProductsApi.Services;

public class TaskService : ITaskService
{
    private static readonly List<TaskItem> Tasks = new()
    {
        new TaskItem { Id = 1, Title = "Task One - meeting notes", IsCompleted = false, DueDate = null, CreatedAt = DateTime.UtcNow.AddDays(-2) },
        new TaskItem { Id = 2, Title = "Task Two", IsCompleted = true, DueDate = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow.AddDays(-1) },
        new TaskItem { Id = 3, Title = "Prepare meeting agenda", IsCompleted = false, DueDate = DateTime.UtcNow.AddDays(3), CreatedAt = DateTime.UtcNow }
    };

    // Whitelist of allowed sort fields. Unknown sortBy values fall back to the default.
    private static readonly Dictionary<string, Func<TaskItem, object?>> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        ["title"] = t => t.Title,
        ["duedate"] = t => t.DueDate,
        ["createdat"] = t => t.CreatedAt,
        ["isCompleted"] = t => t.IsCompleted
    };

    private const string DefaultSortField = "createdat";

    public PagedResult<TaskItem> GetAll(TaskFilterParams filterParams)
    {
        var query = Tasks.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filterParams.Search))
        {
            query = query.Where(t => t.Title.Contains(filterParams.Search, StringComparison.OrdinalIgnoreCase));
        }

        if (filterParams.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == filterParams.IsCompleted.Value);
        }

        var sortField = filterParams.SortBy is not null && SortFields.ContainsKey(filterParams.SortBy)
            ? filterParams.SortBy
            : DefaultSortField;

        query = query.OrderBy(SortFields[sortField]);

        var totalCount = query.Count();

        var page = filterParams.Page < 1 ? 1 : filterParams.Page;
        var pageSize = filterParams.PageSize < 1 ? 1 : filterParams.PageSize;

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

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
}
