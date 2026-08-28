using ProductsApi.Models;

namespace ProductsApi.Services;

public interface ITaskService
{
    Task<PagedResult<TaskItem>> GetAllAsync(TaskFilterParams filterParams);
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(int id, TaskItem task);
    Task<bool> DeleteAsync(int id);
}
