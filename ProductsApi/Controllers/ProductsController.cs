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

    // seed two products for easier testing
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
        return NotFound();
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product? product)
    {
        if (product == null)
            return BadRequest();
        if (string.IsNullOrWhiteSpace(product.Name))
            return BadRequest(new { error = "Name is required" });

        var id = Interlocked.Increment(ref _nextId);
        product.Id = id;
        _store[id] = product;

        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public IActionResult Replace(int id, [FromBody] Product? product)
    {
        if (product == null)
            return BadRequest();
        if (!_store.ContainsKey(id))
            return NotFound();

        product.Id = id;
        _store[id] = product;
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_store.TryRemove(id, out _))
            return NoContent();
        return NotFound();
    }

   
    public class NameDto { public string? Name { get; set; } }

    [HttpPatch("{id:int}")]
    public IActionResult PatchName(int id, [FromBody] NameDto? dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { error = "Name is required" });

        if (!_store.TryGetValue(id, out var existing))
            return NotFound();

        existing.Name = dto.Name;
        _store[id] = existing;
        return Ok(existing);
    }
}
