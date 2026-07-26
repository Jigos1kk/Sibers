using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Services
{
    /// <summary>
    /// Service implementing business logic for project task management.
    /// Handles task creation, retrieval, update, and deletion within projects.
    /// </summary>
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTaskService"/> class.
        /// </summary>
        /// <param name="taskRepository">Repository for task data access.</param>
        /// <param name="employeeRepository">Repository for employee data access.</param>
        /// <param name="projectRepository">Repository for project data access.</param>
        public ProjectTaskService(
            IProjectTaskRepository taskRepository,
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// Creates a new task within a project with the specified details.
        /// Validates that the author, assignee, and project exist before creating the task.
        /// </summary>
        /// <param name="request">The task creation data including name, comment, priority, status, and related entities.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created <see cref="ProjectTask"/> entity.</returns>
        public async Task<ProjectTask> CreateAsync(ProjectTaskRequestDto request, CancellationToken ct = default)
        {
            var author = await _employeeRepository.GetByIdAsync(request.AuthorId, ct);
            var assigned = await _employeeRepository.GetByIdAsync(request.AssignedId, ct);
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, ct);

            var task = new ProjectTask
            {
                Name = request.Name,
                Comment = request.Comment,
                Priority = request.Priority,
                Status = request.Status,
                AuthorID = request.AuthorId,
                AssignedId = request.AssignedId,
                ProjectId = request.ProjectId
            };

            await _taskRepository.AddAsync(task, ct);
            return task;
        }

        /// <summary>
        /// Retrieves all tasks, optionally filtered by project ID.
        /// </summary>
        /// <param name="projectId">Optional project ID to filter tasks. If null, returns all tasks.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of <see cref="ProjectTask"/> entities.</returns>
        public async Task<List<ProjectTask>> ReadAsync(int? projectId, CancellationToken ct = default)
        {
            return projectId.HasValue
                ? await _taskRepository.GetByProjectIdAsync(projectId.Value, ct)
                : await _taskRepository.GetAllAsync(ct);
        }

        /// <summary>
        /// Retrieves a specific task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="ProjectTask"/> entity.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the task is not found.</exception>
        public async Task<ProjectTask> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _taskRepository.GetByIdAsync(id, ct);
        }

        /// <summary>
        /// Updates an existing task with the specified changes.
        /// </summary>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="request">The updated task data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the task is not found.</exception>
        public async Task UpdateAsync(int id, ProjectTaskRequestDto request, CancellationToken ct = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, ct);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            task.Name = request.Name;
            task.Comment = request.Comment;
            task.Priority = request.Priority;
            task.Status = request.Status;
            task.AuthorID = request.AuthorId;
            task.AssignedId = request.AssignedId;

            await _taskRepository.UpdateAsync(task, ct);
        }

        /// <summary>
        /// Deletes a task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the task is not found.</exception>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, ct);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            await _taskRepository.DeleteAsync(task, ct);
        }
    }
}