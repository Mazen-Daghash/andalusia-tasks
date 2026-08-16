using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace ProductsApi.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v2/[controller]")]
public class TasksV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var tasks = new[] {
            new { id = 1, title = "Task One", status = "pending", dueDate = (DateTime?)null, createdAt = DateTime.UtcNow },
            new { id = 2, title = "Task Two", status = "completed", dueDate = (DateTime?)DateTime.UtcNow.AddDays(1), createdAt = DateTime.UtcNow }
        };
        return Ok(tasks);
    }
}
