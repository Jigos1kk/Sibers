using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Services
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;

        public ProjectTaskService(
            IProjectTaskRepository taskRepository,
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
        }

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

        public async Task<List<ProjectTask>> ReadAsync(int? projectId, CancellationToken ct = default)
        {
            return projectId.HasValue
                ? await _taskRepository.GetByProjectIdAsync(projectId.Value, ct)
                : await _taskRepository.GetAllAsync(ct);
        }

        public async Task<ProjectTask> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _taskRepository.GetByIdAsync(id, ct);
        }

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

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, ct);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            await _taskRepository.DeleteAsync(task, ct);
        }
    }
}