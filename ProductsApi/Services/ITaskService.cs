using ProductsApi.Models;

namespace ProductsApi.Services;

public interface ITaskService
{
    Task<PagedResult<TaskItemDto>> GetAllAsync(TaskFilterParams filterParams);
    Task<TaskItemDto?> GetByIdAsync(int id);
    Task<TaskItemDto> CreateAsync(CreateTaskRequest request);
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(int id);
}
