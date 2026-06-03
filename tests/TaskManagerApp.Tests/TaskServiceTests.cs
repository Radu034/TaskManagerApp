using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.API.Data;
using TaskManagerApp.API.DTOs;
using TaskManagerApp.API.Models;
using TaskManagerApp.API.Services;
using ModelTaskStatus = TaskManagerApp.API.Models.TaskStatus;

namespace TaskManagerApp.Tests;

public class TaskServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<AppUser> SeedUserAsync(AppDbContext ctx)
    {
        var user = new AppUser { Email = "test@test.com", Name = "Test", PasswordHash = "hash" };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUserTasks_WhenUserIdProvided()
    {
        using var ctx = CreateContext();
        var user1 = await SeedUserAsync(ctx);
        var user2 = new AppUser { Email = "other@test.com", Name = "Other", PasswordHash = "hash" };
        ctx.Users.Add(user2);
        await ctx.SaveChangesAsync();

        ctx.Tasks.AddRange(
            new TaskItem { Title = "User1 Task", UserId = user1.Id },
            new TaskItem { Title = "User2 Task", UserId = user2.Id }
        );
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);
        var result = await service.GetAllAsync(user1.Id, null, null);

        result.Should().HaveCount(1);
        result.First().Title.Should().Be("User1 Task");
    }

    [Fact]
    public async Task GetAllAsync_FiltersByStatus_WhenStatusProvided()
    {
        using var ctx = CreateContext();
        var user = await SeedUserAsync(ctx);

        ctx.Tasks.AddRange(
            new TaskItem { Title = "Todo Task", UserId = user.Id, Status = ModelTaskStatus.Todo },
            new TaskItem { Title = "Done Task", UserId = user.Id, Status = ModelTaskStatus.Done }
        );
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);
        var result = await service.GetAllAsync(user.Id, ModelTaskStatus.Todo, null);

        result.Should().HaveCount(1);
        result.First().Status.Should().Be(ModelTaskStatus.Todo);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByPriority_WhenPriorityProvided()
    {
        using var ctx = CreateContext();
        var user = await SeedUserAsync(ctx);

        ctx.Tasks.AddRange(
            new TaskItem { Title = "High Task", UserId = user.Id, Priority = TaskPriority.High },
            new TaskItem { Title = "Low Task", UserId = user.Id, Priority = TaskPriority.Low }
        );
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);
        var result = await service.GetAllAsync(user.Id, null, TaskPriority.High);

        result.Should().HaveCount(1);
        result.First().Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTask_WhenExistsAndBelongsToUser()
    {
        using var ctx = CreateContext();
        var user = await SeedUserAsync(ctx);
        var task = new TaskItem { Title = "My Task", UserId = user.Id };
        ctx.Tasks.Add(task);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);
        var result = await service.GetByIdAsync(task.Id, user.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("My Task");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTaskNotFound()
    {
        using var ctx = CreateContext();
        var service = new TaskService(ctx);
        var result = await service.GetByIdAsync(999, 1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTaskBelongsToOtherUser()
    {
        using var ctx = CreateContext();
        var user1 = await SeedUserAsync(ctx);
        var task = new TaskItem { Title = "User1 Task", UserId = user1.Id };
        ctx.Tasks.Add(task);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);
        var result = await service.GetByIdAsync(task.Id, 999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_CreatesTask_WithCorrectValues()
    {
        using var ctx = CreateContext();
        var user = await SeedUserAsync(ctx);
        var dto = new CreateTaskDto
        {
            Title = "New Task",
            Description = "Description",
            Priority = TaskPriority.High
        };

        var service = new TaskService(ctx);
        var result = await service.CreateAsync(user.Id, dto);

        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Priority.Should().Be(TaskPriority.High);
        result.Status.Should().Be(ModelTaskStatus.Todo);
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenTaskDeleted()
    {
        using var ctx = CreateContext();
        var user = await SeedUserAsync(ctx);
        var task = new TaskItem { Title = "Delete Me", UserId = user.Id };
        ctx.Tasks.Add(task);
        await ctx.SaveChangesAsync();

        var service = new TaskService(ctx);
        var result = await service.DeleteAsync(task.Id, user.Id);

        result.Should().BeTrue();
        ctx.Tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenTaskNotFound()
    {
        using var ctx = CreateContext();
        var service = new TaskService(ctx);
        var result = await service.DeleteAsync(999, 1);
        result.Should().BeFalse();
    }
}
