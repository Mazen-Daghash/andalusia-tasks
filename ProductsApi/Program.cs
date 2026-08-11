using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

// Register global exception middleware as the first item in the pipeline
app.UseMiddleware<ProductsApi.Middleware.GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
