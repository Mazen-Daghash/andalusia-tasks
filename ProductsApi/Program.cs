using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Asp.Versioning;
using ProductsApi.Data;
using ProductsApi.Repositories;
using ProductsApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddAutoMapper(cfg => { }, typeof(ProductsApi.MappingProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
