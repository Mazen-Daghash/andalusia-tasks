using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using System.Collections.Concurrent;
using System.Threading;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    
    private static readonly ConcurrentDictionary<int, Product> _store = new();
    private static int _nextId = 0;

    
    static ProductsController()
    {
        var p1 = new Product { Id = Interlocked.Increment(ref _nextId), Name = "Apple", Price = 1.25m };
        var p2 = new Product { Id = Interlocked.Increment(ref _nextId), Name = "Banana", Price = 0.75m };
        _store[p1.Id] = p1;
        _store[p2.Id] = p2;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_store.Values);
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        if (_store.TryGetValue(id, out var product))
            return Ok(product);
        throw new ProductsApi.Errors.NotFoundException($"Product with id {id} not found.");
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product? product)
    {
        if (product == null)
            throw new ProductsApi.Errors.BadRequestException("Request body is required.");
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ProductsApi.Errors.BadRequestException("Name is required.");

        if (_store.Values.Any(p => string.Equals(p.Name, product.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ProductsApi.Errors.ConflictException($"Product with name '{product.Name}' already exists.");

        if (product.DueDate.HasValue && product.DueDate.Value < DateTime.UtcNow)
            throw new ProductsApi.Errors.DueDateInPastException("Due date cannot be in the past.");

        var id = Interlocked.Increment(ref _nextId);
        product.Id = id;
        _store[id] = product;

        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public IActionResult Replace(int id, [FromBody] Product? product)
    {
        if (product == null)
            throw new ProductsApi.Errors.BadRequestException("Request body is required.");
        if (!_store.ContainsKey(id))
            throw new ProductsApi.Errors.NotFoundException($"Product with id {id} not found.");

        product.Id = id;
        _store[id] = product;
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_store.TryRemove(id, out _))
            return NoContent();
        throw new ProductsApi.Errors.NotFoundException($"Product with id {id} not found.");
    }

   
    public class NameDto { public string? Name { get; set; } }

    [HttpPatch("{id:int}")]
    public IActionResult PatchName(int id, [FromBody] NameDto? dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            throw new ProductsApi.Errors.BadRequestException("Name is required.");

        if (!_store.TryGetValue(id, out var existing))
            throw new ProductsApi.Errors.NotFoundException($"Product with id {id} not found.");

        if (_store.Values.Any(p => p.Id != id && string.Equals(p.Name, dto.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ProductsApi.Errors.ConflictException($"Product with name '{dto.Name}' already exists.");

        existing.Name = dto.Name;
        _store[id] = existing;
        return Ok(existing);
    }
}
