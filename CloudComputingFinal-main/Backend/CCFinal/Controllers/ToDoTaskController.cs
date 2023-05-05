using CCFinal.Data;
using CCFinal.Dtos;
using CCFinal.Mappers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.Controllers;

[EnableCors]
[ApiController]
[Route("api/[controller]")]
public class ToDoTaskController : ControllerBase {
    private readonly CCFinalContext _context;
    private readonly ITodoMapper _todoMapper;
    private readonly ILogger<ToDoTaskController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _ca;

    public ToDoTaskController(CCFinalContext context, ITodoMapper todoMapper, ILogger<ToDoTaskController> logger,
        UserManager<IdentityUser> userManager, IHttpContextAccessor ca) {
        _context = context;
        _todoMapper = todoMapper;
        _logger = logger;
        _userManager = userManager;
        _ca = ca;
    }

    // GET: api/ToDoTask
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ToDoTaskDTO>>> GetToDoTask()
    {
        if (_context.ToDoTask == null)
            return NotFound();

        // Get List by User ID
        if (HttpContext.User.Identity.IsAuthenticated) {
            if (HttpContext.User.Identity.Name is not null) {
                var userId = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                if (userId == null)
                    return NotFound();
                List<ToDoTaskDTO> userTasks = await _context.ToDoTask
                    .Where(x => !x.IsDeleted && x.UserID == Guid.Parse(userId.Id))
                    .Select(x => _todoMapper.TodoTaskToDto(x))
                    .ToListAsync();
                foreach (var task in userTasks) {
                    if (task.DueDate == default)
                        task.DueDate = null;
                }

                return userTasks;
            }

            return BadRequest();
        }

        List<ToDoTaskDTO> tasks = await _context.ToDoTask.Where(x => !x.IsDeleted && x.UserID == default)
            .Select(x => _todoMapper.TodoTaskToDto(x))
            .ToListAsync();
        foreach (var task in tasks) {
            if (task.DueDate == default)
                task.DueDate = null;
        }

        // Return globally shared for account-less
        return tasks;
    }

    // GET: api/ToDoTask/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoTaskDTO>> GetToDoTask(int id)
    {
        if (_context.ToDoTask == null) 
            return NotFound();

        // get the task by ID
        var toDoTask = await _context.ToDoTask.FindAsync(id);
        if (toDoTask == null || toDoTask.IsDeleted)
            return NotFound();

        // Guard the user ID, if user is authenticated
        if ((HttpContext.User.Identity?.IsAuthenticated ?? false) || toDoTask.UserID != default) {
            var user = await _userManager.FindByNameAsync(_ca.HttpContext!.User.Identity!.Name!);
            if (user == null)
                return NotFound();
            if (Guid.Parse(user.Id) != toDoTask.UserID)
                return NotFound();
        }

        var returnTask = _todoMapper.TodoTaskToDto(toDoTask);
        if (toDoTask.DueDate == default)
            returnTask.DueDate = null;

        return Ok(returnTask);
    }

    // PUT: api/ToDoTask/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutToDoTask(int id, ToDoTaskDTO toDoTaskDTO) {
        if (toDoTaskDTO.Id == default)
            toDoTaskDTO.Id = id;
        if (toDoTaskDTO.Id != id)
            return BadRequest();

        // Fetch task by User ID or globally shared, return not found otherwise
        var dbTask = await _context.ToDoTask.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        if (dbTask is null ||
            (dbTask.UserID != default && !(_ca?.HttpContext?.User.Identity?.IsAuthenticated ?? false)))
            return NotFound();
        if ((_ca?.HttpContext?.User.Identity?.IsAuthenticated ?? false) || dbTask.UserID != default) {
            var user = await _userManager.FindByNameAsync(_ca.HttpContext.User.Identity.Name);
            if (user is null || dbTask.UserID != Guid.Parse(user.Id))
                return NotFound();
        }


        // Making sure that the Created and updated fields get transferred correctly
        toDoTaskDTO.Created = dbTask.Created;
        toDoTaskDTO.Updated = DateTime.UtcNow;
        _todoMapper.TodoTaskDtoToModel(toDoTaskDTO, dbTask);

        // Save changes and return
        try {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            if (!ToDoTaskExists(id)) {
                _logger.LogInformation($"Task {id} was queried, but could not be found");
                return NotFound();
            }

            _logger.LogDebug(ex, $"Unable to delete task {dbTask.Id}");
        }

        return NoContent();
    }

    // POST: api/ToDoTask
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ToDoTaskDTO>> PostToDoTask(ToDoTaskDTO toDoTaskDto) {
        if (_context.ToDoTask == null)
            return Problem("Entity set 'CCFinalContext.ToDoTask'  is null.");

        // Convert DTO to Entity
        var toDoTask = new TodoMapper().TodoTaskDtoToModel(toDoTaskDto);
        toDoTask.Created = DateTime.UtcNow;
        toDoTask.Updated = DateTime.UtcNow;
        toDoTask.Id = default;

        // Set UserId
        if (_ca?.HttpContext?.User?.Identity?.IsAuthenticated ?? false) {
            var user = await _userManager.FindByNameAsync(_ca!.HttpContext!.User.Identity!.Name!);
            if (user is null)
                return BadRequest();
            toDoTask.UserID = Guid.Parse(user.Id);
        }

        // Add item to DB
        _context.ToDoTask.Add(toDoTask);
        var result = await _context.SaveChangesAsync();

        if (result < 1)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Unable to create this task." });

        // Convert saved item to DTO 
        var toDoTaskReturn = _todoMapper.TodoTaskToDto(toDoTask);
        return CreatedAtAction("GetToDoTask", new { id = toDoTaskReturn.Id }, toDoTaskReturn);
    }

    // DELETE: api/ToDoTask/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteToDoTask(int id)
    {
        if (_context.ToDoTask == null)
            return NotFound();

        var toDoTask = await _context.ToDoTask.FindAsync(id);

        if (toDoTask == null)
            return NotFound();

        // Check if that user created that task
        if ((_ca?.HttpContext?.User?.Identity?.IsAuthenticated ?? false) || toDoTask.UserID != default) {
            var user = await _userManager.FindByNameAsync(_ca.HttpContext.User.Identity.Name);
            if (user is null || toDoTask.UserID != Guid.Parse(user.Id))
                return NotFound();
        }

        // Checking if it is an integration task
        if (!string.IsNullOrWhiteSpace(toDoTask.IntegrationId)) {
            toDoTask.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Removing task
        _context.ToDoTask.Remove(toDoTask);

        try {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            _logger.LogDebug(ex, $"Unable to remove object {toDoTask?.Id}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Failed", Message = $"Failed to delete task {toDoTask?.Id}" });
        }

        return Ok(new Response { Status = "Success", Message = "Task deleted successfully!" });
    }

    private bool ToDoTaskExists(int id)
    {
        return (_context.ToDoTask?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}