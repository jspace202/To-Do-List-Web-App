using CCFinal.Data;
using CCFinal.Dtos;
using CCFinal.Mappers;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToDoIntegrationController : ControllerBase {
    private readonly CCFinalContext _context;
    private readonly ITodoMapper _todoMapper;

    public ToDoIntegrationController(CCFinalContext context, ITodoMapper todoMapper) {
        _context = context;
        _todoMapper = todoMapper;
    }

    // GET: api/ToDoTask
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ToDoTaskIntegrationDto>>> GetToDoTask() {
        return _context.ToDoTask.Select(x => _todoMapper.TodoTaskModelToDto(x)).ToList();
    }

    // GET: api/ToDoTask/5
    [HttpGet("{IntegrationId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ToDoTaskIntegrationDto>> GetToDoTask(string id) {
        var toDoTask = await _context.ToDoTask.FirstOrDefaultAsync(x => x.IntegrationId == id);

        if (toDoTask == null)
            return NotFound();

        return _todoMapper.TodoTaskModelToDto(toDoTask);
    }

    // PUT: api/ToDoTask/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{IntegrationId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PutToDoTask(string IntegrationId, ToDoTaskIntegrationDto toDoTaskDTO) {
        if (toDoTaskDTO.UserID == default)
            return BadRequest();
        if (toDoTaskDTO.IntegrationId == default)
            toDoTaskDTO.IntegrationId = IntegrationId;
        if (IntegrationId != toDoTaskDTO.IntegrationId)
            return BadRequest();

        var task = await _context.ToDoTask.FirstOrDefaultAsync(x =>
            x.IntegrationId == IntegrationId && x.UserID == toDoTaskDTO.UserID);

        if (task is null)
            return NotFound();

        //
        if (task.Updated > toDoTaskDTO.Updated)
            return BadRequest("The database object is more up to date than the sent object");

        toDoTaskDTO.Id = task.Id;
        toDoTaskDTO.Created = task.Created;
        toDoTaskDTO.IsFavorite = task.IsFavorite;
        toDoTaskDTO.IsCompleted = task.IsCompleted;

        _todoMapper.TodoTaskDtoToModel(toDoTaskDTO, task);

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!ToDoTaskExists(task.Id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/ToDoTask
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [CapSubscribe("IntegrationTaskUpsert")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostToDoTask(ToDoTaskIntegrationDto toDoTaskDto) {
        if (toDoTaskDto.UserID == default || string.IsNullOrWhiteSpace(toDoTaskDto.IntegrationId))
            return BadRequest("Id fields IntegrationId and UserID cannot be null or empty");

        var task = await _context.ToDoTask.AnyAsync(x =>
            x.IntegrationId == toDoTaskDto.IntegrationId && x.UserID == toDoTaskDto.UserID);

        // Creation if the task doesn't exist already
        if (!task) {
            var toDoTask = new TodoMapper().TodoTaskDtoToModel(toDoTaskDto);
            toDoTask.Id = default;
            toDoTask.Created = toDoTask.Updated = DateTime.UtcNow;

            await _context.ToDoTask.AddAsync(toDoTask);
            await _context.SaveChangesAsync();

            _todoMapper.TodoTaskModelToDto(toDoTask, toDoTaskDto);


            return CreatedAtAction("GetToDoTask", new { id = toDoTaskDto.Id }, toDoTaskDto);
        }

        //If the task already exists, Update
        try {
            return await PutToDoTask(toDoTaskDto.IntegrationId, toDoTaskDto);
        }
        catch (Exception ex) {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response {
                    Status = "Error",
                    Message =
                        $"Unable to Upsert the task for User ID: {toDoTaskDto.UserID} and Integration ID: {toDoTaskDto.IntegrationId}"
                });
        }
    }

    // DELETE: api/ToDoTask/5
    [HttpDelete("{IntegrationId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteToDoTask(string id, Guid userId) {
        var toDoTask = await _context.ToDoTask.FirstOrDefaultAsync(x => x.IntegrationId == id && x.UserID == userId);

        if (toDoTask == null)
            return NotFound();

        //Don't delete the object as the system will create a new if the integration fires again on this id
        toDoTask.IsDeleted = true;
        toDoTask.Updated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ToDoTaskExists(int id) {
        return (_context.ToDoTask?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}