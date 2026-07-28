using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class ProjectTaskRepository : BaseRepository<ProjectTask>, IProjectTaskRepository
    {
        public ProjectTaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ProjectTask>> GetByProjectIdAsync(int projectId, CancellationToken ct)
        {
            return await _dbSet
                .Include(t => t.Author)
                .Include(t => t.Assigned)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync(ct);
        }
    }
}