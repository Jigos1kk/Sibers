using Microsoft.AspNetCore.Mvc;
using Moq;
using SibersTest.API.Controllers;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.API.Tests;

public class ProjectControllerTests
{
    private readonly Mock<IProjectService> _mockService;
    private readonly ProjectController _controller;

    public ProjectControllerTests()
    {
        _mockService = new Mock<IProjectService>();
        _controller = new ProjectController(_mockService.Object);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithValidRequest()
    {
        // Arrange
        var request = new ProjectRequestDto
        {
            ProjectName = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            Priority = 1,
            CustomerCompanyName = "Customer",
            ContractorCompanyName = "Contractor",
            ManagerId = 1,
            EmployeeIds = new List<int> { 1, 2 }
        };

        var createdProject = new Project
        {
            Id = 1,
            Name = request.ProjectName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Priority = request.Priority,
            Manager = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            Employes = new List<Employe>(),
            Customer = new Company { Id = 1, Name = request.CustomerCompanyName },
            Contractor = new Company { Id = 2, Name = request.ContractorCompanyName },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProjectController.GetById), createdAtResult.ActionName);
        Assert.Equal(201, createdAtResult.StatusCode);

        var response = Assert.IsType<ProjectResponseDto>(createdAtResult.Value);
        Assert.Equal(createdProject.Id, response.Id);
        Assert.Equal(createdProject.Name, response.Name);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProjectExists()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            Priority = 1,
            Manager = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
            Employes = new List<Employe>(),
            Customer = new Company { Id = 1, Name = "Customer" },
            Contractor = new Company { Id = 2, Name = "Contractor" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.ReadAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ProjectResponseDto>(okResult.Value);
        Assert.Equal(project.Id, response.Id);
        Assert.Equal(project.Name, response.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _mockService
            .Setup(s => s.ReadAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Project not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _controller.GetById(999, CancellationToken.None));
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithListOfProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1, Name = "Project 1", StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1), Priority = 1,
                Manager = new Employe { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" },
                Employes = new List<Employe>(),
                Customer = new Company { Id = 1, Name = "Customer" },
                Contractor = new Company { Id = 2, Name = "Contractor" },
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2, Name = "Project 2", StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(2), Priority = 2,
                Manager = new Employe { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane@test.com" },
                Employes = new List<Employe>(),
                Customer = new Company { Id = 3, Name = "Customer2" },
                Contractor = new Company { Id = 4, Name = "Contractor2" },
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        _mockService
            .Setup(s => s.ReadAsync(It.IsAny<ProjectFilterQueryDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var result = await _controller.GetAll(CancellationToken.None, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<ProjectResponseDto>>(okResult.Value);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenProjectExists()
    {
        // Arrange
        var request = new ProjectRequestDto
        {
            ProjectName = "Updated Project",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(2),
            Priority = 2,
            CustomerCompanyName = "Updated Customer",
            ContractorCompanyName = "Updated Contractor",
            ManagerId = 1,
            EmployeeIds = new List<int> { 1 }
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
    public async Task Delete_ReturnsOk_WhenProjectExists()
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
    public async Task Delete_ThrowsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _mockService
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Project not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _controller.Delete(999, CancellationToken.None));
    }
}