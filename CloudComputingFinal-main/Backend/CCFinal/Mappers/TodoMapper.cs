using CCFinal.Dtos;
using CCFinal.Entities;
using Riok.Mapperly.Abstractions;

namespace CCFinal.Mappers;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public partial class TodoMapper : ITodoMapper {
    public partial ToDoTask TodoTaskDtoToModel(ToDoTaskDTO task);
    public partial void TodoTaskDtoToModel(ToDoTaskDTO task, ToDoTask model);
    public partial ToDoTaskDTO TodoTaskToDto(ToDoTask model);
    public partial void TodoTaskToDto(ToDoTask model, ToDoTaskDTO dto);
    public partial ToDoTask TodoTaskDtoToModel(ToDoTaskIntegrationDto dto);
    public partial void TodoTaskDtoToModel(ToDoTaskIntegrationDto dto, ToDoTask model);

    public partial ToDoTaskIntegrationDto TodoTaskModelToDto(ToDoTask task);
    public partial void TodoTaskModelToDto(ToDoTask model, ToDoTaskIntegrationDto task);
}

public interface ITodoMapper {
    public ToDoTask TodoTaskDtoToModel(ToDoTaskDTO task);
    public void TodoTaskDtoToModel(ToDoTaskDTO task, ToDoTask model);
    public ToDoTaskDTO TodoTaskToDto(ToDoTask model);
    public void TodoTaskToDto(ToDoTask model, ToDoTaskDTO dto);
    public ToDoTask TodoTaskDtoToModel(ToDoTaskIntegrationDto task);
    public void TodoTaskDtoToModel(ToDoTaskIntegrationDto task, ToDoTask model);
    public ToDoTaskIntegrationDto TodoTaskModelToDto(ToDoTask task);
    public void TodoTaskModelToDto(ToDoTask model, ToDoTaskIntegrationDto task);
}