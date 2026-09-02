using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ProductsApi.Models;

namespace ProductsApi.Authorization;

// Users can only manage (edit/delete) tasks they own; Admins bypass the ownership check.
public class TaskAuthorizationHandler : AuthorizationHandler<CanManageTasksRequirement, TaskItem>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanManageTasksRequirement requirement,
        TaskItem resource)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is not null && int.Parse(userId) == resource.UserId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
