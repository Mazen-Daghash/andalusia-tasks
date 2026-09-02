using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;
using ProductsApi.Repositories;

namespace ProductsApi.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // Whitelist of allowed sort fields. Unknown sortBy values fall back to the default.
    private const string DefaultSortField = "createdat";

    public async Task<PagedResult<TaskItemDto>> GetAllAsync(TaskFilterParams filterParams, int userId)
    {
        var query = _repository.Query().Where(t => t.UserId == userId);

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
            .ProjectTo<TaskItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<TaskItemDto>
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

    public async Task<TaskItemDto?> GetByIdAsync(int id, int userId)
    {
        var task = await _repository.GetByIdAsync(id);
        return task is null || task.UserId != userId ? null : _mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskRequest request, int userId)
    {
        var task = _mapper.Map<TaskItem>(request);
        task.UserId = userId;
        await _repository.AddAsync(task);
        return _mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskRequest request, int userId)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
        {
            return null;
        }

        _mapper.Map(request, existing);
        await _repository.SaveChangesAsync();
        return _mapper.Map<TaskItemDto>(existing);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null || existing.UserId != userId)
        {
            return false;
        }

        _repository.Remove(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}
