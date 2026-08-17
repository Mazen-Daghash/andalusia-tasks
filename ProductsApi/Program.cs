using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Asp.Versioning;
using ProductsApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskService>();

// Configure API versioning as requested with default version 1.0
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

// Middleware to inject API versioning headers (supported, deprecated, Sunset for bonus)
app.UseMiddleware<ProductsApi.Middleware.ApiVersioningHeadersMiddleware>();

// Global exception middleware
app.UseMiddleware<ProductsApi.Middleware.GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
