using TaskManagerApp.API.DTOs;
using TaskManagerApp.API.Models;
using ModelTaskStatus = TaskManagerApp.API.Models.TaskStatus;

namespace TaskManagerApp.API.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllAsync(int userId, ModelTaskStatus? status, TaskPriority? priority);
    Task<TaskItem?> GetByIdAsync(int id, int userId);
    Task<TaskItem> CreateAsync(int userId, CreateTaskDto dto);
    Task<TaskItem?> UpdateAsync(int id, int userId, UpdateTaskDto dto);
    Task<bool> DeleteAsync(int id, int userId);
}
