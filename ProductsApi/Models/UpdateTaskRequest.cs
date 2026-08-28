namespace ProductsApi.Models;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public int UserId { get; set; }
}
