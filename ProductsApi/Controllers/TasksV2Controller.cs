using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using ProductsApi.Authorization;
using ProductsApi.Models;
using ProductsApi.Services;

namespace ProductsApi.Controllers;

[ApiController]
[Authorize]
[ApiVersion("2.0")]
[Route("api/v2/[controller]")]
public class TasksV2Controller : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IAuthorizationService _authorizationService;

    public TasksV2Controller(ITaskService taskService, IAuthorizationService authorizationService)
    {
        _taskService = taskService;
        _authorizationService = authorizationService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TaskFilterParams filterParams)
    {
        var result = await _taskService.GetAllAsync(filterParams, CurrentUserId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id, CurrentUserId);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var created = await _taskService.CreateAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
    {
        var entity = await _taskService.GetEntityByIdAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        var authResult = await _authorizationService.AuthorizeAsync(User, entity, "CanManageTasks");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        var updated = await _taskService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _taskService.GetEntityByIdAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        var authResult = await _authorizationService.AuthorizeAsync(User, entity, "CanManageTasks");
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        await _taskService.DeleteAsync(id);
        return NoContent();
    }
}
