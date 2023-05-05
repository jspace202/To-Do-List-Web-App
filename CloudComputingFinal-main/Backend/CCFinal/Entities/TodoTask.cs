using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.Entities;

[PrimaryKey(nameof(Id))]
public class ToDoTask {
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    public TaskType TaskType { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsFavorite { get; set; }

    [Key]
    public Guid UserID { get; set; }

    [Key]
    public string? IntegrationId { get; set; }

    public bool IsDeleted { get; set; }
}