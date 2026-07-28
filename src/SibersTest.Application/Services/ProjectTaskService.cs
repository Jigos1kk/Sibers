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
    /// All write operations are wrapped in a single transaction to ensure data consistency.
    /// </summary>
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectTaskService(
            IProjectTaskRepository taskRepository,
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new task within a project with the specified details within a single transaction.
        /// Validates that the author, assignee, and project exist before creating the task.
        /// </summary>
        public async Task<ProjectTask> CreateAsync(ProjectTaskRequestDto request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
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
                await _unitOfWork.CommitAsync(ct);
                return task;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all tasks, optionally filtered by project ID.
        /// </summary>
        public async Task<List<ProjectTask>> ReadAsync(int? projectId, CancellationToken ct = default)
        {
            return projectId.HasValue
                ? await _taskRepository.GetByProjectIdAsync(projectId.Value, ct)
                : await _taskRepository.GetAllAsync(ct);
        }

        /// <summary>
        /// Retrieves a specific task by its unique identifier.
        /// </summary>
        public async Task<ProjectTask> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _taskRepository.GetByIdAsync(id, ct);
        }

        /// <summary>
        /// Updates an existing task with the specified changes within a single transaction.
        /// </summary>
        public async Task UpdateAsync(int id, ProjectTaskRequestDto request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
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
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Deletes a task by its unique identifier within a single transaction.
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var task = await _taskRepository.GetByIdAsync(id, ct);

                if (task == null)
                    throw new KeyNotFoundException($"Task with ID {id} not found.");

                await _taskRepository.DeleteAsync(task, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}