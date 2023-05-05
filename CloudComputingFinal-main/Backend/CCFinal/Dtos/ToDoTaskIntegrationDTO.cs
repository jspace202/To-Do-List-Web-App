namespace CCFinal.Dtos;

public class ToDoTaskIntegrationDto {
    public int Id { get; set; }
    public string IntegrationId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public Guid UserID { get; set; }
    public TaskTypeDTO TaskType { get; set; }
    public bool IsFavorite {get; set; }
    public bool IsCompleted { get; set; }
}