namespace CCFinal.CanvasIntegration.PublishEvents;

public record PostUpdate { }

public record ToDoTaskIntegrationDto(int Id,
    string IntegrationId,
    DateTime Created,
    DateTime? DueDate,
    Guid UserID,
    TaskTypeDTO TaskType,
    bool IsFavorite,
    bool IsCompleted,
    string Title,
    string Description,
    DateTime Updated);

public enum TaskTypeDTO {
    Task,
    Quiz,
    Assignment,
    Discussion
}