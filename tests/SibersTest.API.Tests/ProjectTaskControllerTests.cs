using Microsoft.AspNetCore.Mvc;
using Moq;
using SibersTest.API.Controllers;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;
using SibersTest.Domain.Enum;

namespace SibersTest.API.Tests;

public class ProjectTaskControllerTests
{
    private readonly Mock<IProjectTaskService> _mockService;
    private readonly ProjectTaskController _controller;

    public ProjectTaskControllerTests()
    {
        _mockService = new Mock<IProjectTaskService>();
        _controller = new ProjectTaskController(_mockService.Object);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithValidRequest()
    {
        // Arrange
        var request = new ProjectTaskRequestDto
        {
            Name = "Test Task",
            Comment = "Test comment",
            Priority = 1,
            Status = ProjectTaskStatus.ToDo,
            AuthorId = 1,
            AssignedId = 2,
            ProjectId = 1
        };

        var createdTask = new ProjectTask
        {
            Id = 1,
            Name = request.Name,
            Comment = request.Comment,
            Priority = request.Priority,
            Status = request.Status,
            AuthorID = request.AuthorId,
            AssignedId = request.AssignedId,
            ProjectId = request.ProjectId,
            Author = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            Assigned = new Employe { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProjectTaskController.GetById), createdAtResult.ActionName);
        Assert.Equal(201, createdAtResult.StatusCode);

        var response = Assert.IsType<ProjectTaskResponseDto>(createdAtResult.Value);
        Assert.Equal(createdTask.Id, response.Id);
        Assert.Equal(createdTask.Name, response.Name);
        Assert.Equal(createdTask.Status, response.Status);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenTaskExists()
    {
        // Arrange
        var task = new ProjectTask
        {
            Id = 1,
            Name = "Test Task",
            Priority = 1,
            Status = ProjectTaskStatus.ToDo,
            AuthorID = 1,
            AssignedId = 2,
            ProjectId = 1,
            Author = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            Assigned = new Employe { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.ReadAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ProjectTaskResponseDto>(okResult.Value);
        Assert.Equal(task.Id, response.Id);
        Assert.Equal(task.Name, response.Name);
    }

    [Fact]
    public async Task GetById_ThrowsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        _mockService
            .Setup(s => s.ReadAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Task not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _controller.GetById(999, CancellationToken.None));
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithListOfTasks()
    {
        // Arrange
        var tasks = new List<ProjectTask>
        {
            new()
            {
                Id = 1, Name = "Task 1", Priority = 1, Status = ProjectTaskStatus.ToDo,
                AuthorID = 1, AssignedId = 2, ProjectId = 1,
                Author = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
                Assigned = new Employe { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" },
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2, Name = "Task 2", Priority = 2, Status = ProjectTaskStatus.Progress,
                AuthorID = 1, AssignedId = 3, ProjectId = 1,
                Author = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
                Assigned = new Employe { Id = 3, FirstName = "Bob", LastName = "Brown", Email = "bob@test.com" },
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        _mockService
            .Setup(s => s.ReadAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAll(null, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ProjectTaskResponseDto>>(okResult.Value);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task GetAll_WithProjectId_ReturnsFilteredTasks()
    {
        // Arrange
        var tasks = new List<ProjectTask>
        {
            new()
            {
                Id = 1, Name = "Task 1", Priority = 1, Status = ProjectTaskStatus.ToDo,
                AuthorID = 1, AssignedId = 2, ProjectId = 1,
                Author = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
                Assigned = new Employe { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" },
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        _mockService
            .Setup(s => s.ReadAsync(It.Is<int?>(x => x == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAll(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ProjectTaskResponseDto>>(okResult.Value);
        Assert.Single(response);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenTaskExists()
    {
        // Arrange
        var request = new ProjectTaskRequestDto
        {
            Name = "Updated Task",
            Comment = "Updated comment",
            Priority = 2,
            Status = ProjectTaskStatus.Progress,
            AuthorId = 1,
            AssignedId = 2,
            ProjectId = 1
        };

        _mockService
            .Setup(s => s.UpdateAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenTaskExists()
    {
        // Arrange
        _mockService
            .Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_ThrowsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        _mockService
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Task not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _controller.Delete(999, CancellationToken.None));
    }
}