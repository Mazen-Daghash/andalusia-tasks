using ProductsApi.Models;

namespace ProductsApi.Services;

public interface ITaskService
{
    PagedResult<TaskItem> GetAll(TaskFilterParams filterParams);
}
