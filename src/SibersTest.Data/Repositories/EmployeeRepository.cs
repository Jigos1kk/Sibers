using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Employe employee, CancellationToken ct)
        {
            await _context.Employees.AddAsync(employee, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Employe employee, CancellationToken ct)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Employe>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Employees
                .Include(e => e.ManagedProjects)
                .Include(e => e.AssignedTasks)
                .Include(e => e.AuthoredTasks)
                .Include(e => e.Projects)
                .ToListAsync(ct);
        }

        public async Task<Employe> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Employees
                .Include(e => e.ManagedProjects)
                .Include(e => e.AssignedTasks)
                .Include(e => e.AuthoredTasks)
                .Include(e => e.Projects)
                .FirstOrDefaultAsync(e => e.Id == id ,ct)
                ?? throw new InvalidOperationException($"Not found Employee with ID {id}");
        }

        public async Task<List<Employe>> SearchAsync(string term, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<Employe>();

            return await _context.Employees
                .Where(e =>
                    e.LastName.Contains(term) ||
                    e.FirstName.Contains(term) ||
                    (e.MiddleName != null && e.MiddleName.Contains(term)))
                .Take(20)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Employe employee, CancellationToken ct)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync(ct);
        }
    }
}