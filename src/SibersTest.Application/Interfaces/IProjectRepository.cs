using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> GetByIdAsync(int id, CancellationToken ct);
        Task<List<Project>> GetAllAsync(CancellationToken ct);
        Task<List<Project>> GetFillterAsync(ProjectFilterQueryDto filter, CancellationToken ct);
        Task AddAsync(Project project, CancellationToken ct);
        Task UpdateAsync(Project project, CancellationToken ct);
        Task DeleteAsync(Project project, CancellationToken ct);
    }
}