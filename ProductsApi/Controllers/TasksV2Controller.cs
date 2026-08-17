using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using ProductsApi.Models;
using ProductsApi.Services;

namespace ProductsApi.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v2/[controller]")]
public class TasksV2Controller : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksV2Controller(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] TaskFilterParams filterParams)
    {
        var result = _taskService.GetAll(filterParams);
        return Ok(result);
    }
}
