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
                Manager = project.Manager,
                Employes = project.Employes,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }
    }
}