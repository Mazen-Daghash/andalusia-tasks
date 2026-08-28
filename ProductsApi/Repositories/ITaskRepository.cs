using ProductsApi.Models;

namespace ProductsApi.Repositories;

public interface ITaskRepository
{
    IQueryable<TaskItem> Query();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> AddAsync(TaskItem task);
    Task<bool> SaveChangesAsync();
    void Remove(TaskItem task);
}
