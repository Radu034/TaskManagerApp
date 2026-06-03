using FluentAssertions;
using Xunit;
using TaskManagerApp.API.Models;
using ModelTaskStatus = TaskManagerApp.API.Models.TaskStatus;

namespace TaskManagerApp.Tests;

public class TaskItemModelTests
{
    [Fact]
    public void TaskItem_DefaultStatus_IsTodo()
    {
        var task = new TaskItem();
        task.Status.Should().Be(ModelTaskStatus.Todo);
    }

    [Fact]
    public void TaskItem_DefaultPriority_IsMedium()
    {
        var task = new TaskItem();
        task.Priority.Should().Be(TaskPriority.Medium);
    }
}
