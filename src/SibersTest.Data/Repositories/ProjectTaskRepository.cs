using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProjectTask task, CancellationToken ct)
        {
            await _context.ProjectTasks.AddAsync(task, ct);
        }

        public Task UpdateAsync(ProjectTask task, CancellationToken ct)
        {
            _context.ProjectTasks.Update(task);
            return Task.CompletedTask;
        }

        public async Task<List<ProjectTask>> GetAllAsync(CancellationToken ct)
        {
            return await _context.ProjectTasks
                .Include(t => t.Author)
                .Include(t => t.Assigned)
                .Include(t => t.Project)
                .ToListAsync(ct);
        }

        public async Task<List<ProjectTask>> GetByProjectIdAsync(int projectId, CancellationToken ct)
        {
            return await _context.ProjectTasks
                .Include(t => t.Author)
                .Include(t => t.Assigned)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync(ct);
        }

        public async Task<ProjectTask> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.ProjectTasks
                .Include(t => t.Author)
                .Include(t => t.Assigned)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id, ct)
                ?? throw new InvalidOperationException($"Not found task with ID {id}");
        }

        public Task DeleteAsync(ProjectTask task, CancellationToken ct)
        {
            _context.ProjectTasks.Remove(task);
            return Task.CompletedTask;
        }
    }
}