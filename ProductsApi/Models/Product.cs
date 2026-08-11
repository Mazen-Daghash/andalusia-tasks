namespace ProductsApi.Models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    // optional due date used for the bonus validation
    public DateTime? DueDate { get; set; }
}
