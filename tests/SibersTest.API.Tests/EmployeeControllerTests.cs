using Microsoft.AspNetCore.Mvc;
using Moq;
using SibersTest.API.Controllers;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.API.Tests;

public class EmployeeControllerTests
{
    private readonly Mock<IEmployeeService> _mockService;
    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        _mockService = new Mock<IEmployeeService>();
        _controller = new EmployeeController(_mockService.Object);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithValidRequest()
    {
        // Arrange
        var request = new EmployeeRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "Middle",
            Email = "john.doe@test.com"
        };

        var createdEmployee = new Employe
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEmployee);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(EmployeeController.GetById), createdAtResult.ActionName);
        Assert.Equal(201, createdAtResult.StatusCode);

        var response = Assert.IsType<EmployeeResponseDto>(createdAtResult.Value);
        Assert.Equal(createdEmployee.Id, response.Id);
        Assert.Equal(createdEmployee.FirstName, response.FirstName);
        Assert.Equal(createdEmployee.LastName, response.LastName);
        Assert.Equal(createdEmployee.Email, response.Email);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employe
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(s => s.ReadAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EmployeeResponseDto>(okResult.Value);
        Assert.Equal(employee.Id, response.Id);
        Assert.Equal(employee.FirstName, response.FirstName);
        Assert.Equal(employee.LastName, response.LastName);
    }

    [Fact]
    public async Task GetById_ThrowsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        _mockService
            .Setup(s => s.ReadAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Employee not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _controller.GetById(999, CancellationToken.None));
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithListOfEmployees()
    {
        // Arrange
        var employees = new List<Employe>
        {
            new()
            {
                Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        _mockService
            .Setup(s => s.ReadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<List<EmployeeResponseDto>>(okResult.Value);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenEmployeeExists()
    {
        // Arrange
        var request = new EmployeeRequestDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@test.com"
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
    public async Task Delete_ReturnsOk_WhenEmployeeExists()
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
    public async Task Delete_ThrowsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        _mockService
            .Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Employee not found"));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _controller.Delete(999, CancellationToken.None));
    }
}