using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace ProductsApi.Controllers;

[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[Route("api/v1/[controller]")]
public class TasksV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var tasks = new[] {
            new { id = 1, title = "Task One", isCompleted = false },
            new { id = 2, title = "Task Two", isCompleted = true }
        };
        return Ok(tasks);
    }
}
