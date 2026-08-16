using Microsoft.AspNetCore.Http;

namespace ProductsApi.Middleware;

public class ApiVersioningHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public ApiVersioningHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // Add supported versions header on every response
        context.Response.Headers["api-supported-versions"] = "1.0, 2.0";

        // Add deprecated versions header only for v1
        if (context.Request.Path.Value?.Contains("/api/v1/") == true)
        {
            context.Response.Headers["api-deprecated-versions"] = "1.0";
            // Bonus: add Sunset header one year from now for deprecated v1
            var sunset = DateTime.UtcNow.AddYears(1).ToString("R");
            context.Response.Headers["Sunset"] = sunset;
        }
    }
}
