using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Project project, CancellationToken ct)
        {
            await _context.Projects.AddAsync(project, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Project project, CancellationToken ct)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Project>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Contractor)
                .Include(p => p.Manager)
                .Include(p => p.Employes)
                .ToListAsync(ct);
        }

        public async Task<Project> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Contractor)
                .Include(p => p.Manager)
                .Include(p => p.Employes)
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new InvalidOperationException($"Not found project with ID {id}");
        }

        public async Task DeleteAsync(Project project, CancellationToken ct)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(ct);
        }
    }
}