using System.Data.Common;
using System.Linq;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.DTOs
{
    public static class MapToResponseDto
    {
        public static ProjectResponseDto MapProject(Project project)
        {
            return new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,
                CustomerCompanyName = project.Customer?.Name ?? string.Empty,
                ContractorCompanyName = project.Contractor?.Name ?? string.Empty,
                Manager = project.Manager != null ? MapEmployee(project.Manager) : null!,
                Employes = project.Employes?.Select(e => MapEmployee(e)).ToList() ?? new List<EmployeeResponseDto>(),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        public static EmployeeResponseDto MapEmployee(Employe employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                Email = employee.Email,
            };
        }

        public static ProjectTaskResponseDto MapTask(ProjectTask task)
        {
            return new ProjectTaskResponseDto
            {
                Id = task.Id,
                Name = task.Name,
                Comment = task.Comment,
                Priority = task.Priority,
                Status = task.Status,
                Author = task.Author != null ? MapEmployee(task.Author) : null!,
                Assigned = task.Assigned != null ? MapEmployee(task.Assigned) : null!,
                ProjectId = task.ProjectId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}