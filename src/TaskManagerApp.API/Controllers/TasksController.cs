using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApp.API.DTOs;
using TaskManagerApp.API.Models;
using TaskManagerApp.API.Services;
using ModelTaskStatus = TaskManagerApp.API.Models.TaskStatus;

namespace TaskManagerApp.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ModelTaskStatus? status, [FromQuery] TaskPriority? priority)
    {
        var tasks = await _taskService.GetAllAsync(GetUserId(), status, priority);
        return Ok(tasks.Select(TaskResponseDto.FromTaskItem));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id, GetUserId());
        if (task is null) return NotFound();
        return Ok(TaskResponseDto.FromTaskItem(task));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var task = await _taskService.CreateAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, TaskResponseDto.FromTaskItem(task));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _taskService.UpdateAsync(id, GetUserId(), dto);
        if (task is null) return NotFound();
        return Ok(TaskResponseDto.FromTaskItem(task));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(id, GetUserId());
        if (!deleted) return NotFound();
        return NoContent();
    }
}
