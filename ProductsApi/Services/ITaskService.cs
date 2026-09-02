using ProductsApi.Models;

namespace ProductsApi.Services;

public interface ITaskService
{
    Task<PagedResult<TaskItemDto>> GetAllAsync(TaskFilterParams filterParams, int userId);
    Task<TaskItemDto?> GetByIdAsync(int id, int userId);
    Task<TaskItem?> GetEntityByIdAsync(int id);
    Task<TaskItemDto> CreateAsync(CreateTaskRequest request, int userId);
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(int id);
}
