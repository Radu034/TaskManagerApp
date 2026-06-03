using TaskManagerApp.API.Models;
using ModelTaskStatus = TaskManagerApp.API.Models.TaskStatus;

namespace TaskManagerApp.API.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public ModelTaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
}
