using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface IProjectTaskRepository
    {
        Task<ProjectTask> GetByIdAsync(int id, CancellationToken ct, params Expression<Func<ProjectTask, object>>[] includes);
        Task<List<ProjectTask>> GetAllAsync(CancellationToken ct, params Expression<Func<ProjectTask, object>>[] includes);
        Task<List<ProjectTask>> GetByProjectIdAsync(int projectId, CancellationToken ct);
        Task AddAsync(ProjectTask task, CancellationToken ct);
        Task UpdateAsync(ProjectTask task, CancellationToken ct);
        Task DeleteAsync(ProjectTask task, CancellationToken ct);
    }
}