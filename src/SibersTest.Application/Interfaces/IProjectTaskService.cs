using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface IProjectTaskService
    {
        Task<ProjectTask> CreateAsync(ProjectTaskRequestDto request, CancellationToken ct = default);
        Task<List<ProjectTask>> ReadAsync(int? projectId, CancellationToken ct = default);
        Task<ProjectTask> ReadAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, ProjectTaskRequestDto request, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}