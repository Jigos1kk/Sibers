using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class EmployeeRepository : BaseRepository<Employe>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Employe>> SearchAsync(string term, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<Employe>();

            return await _dbSet
                .Where(e =>
                    e.LastName.Contains(term) ||
                    e.FirstName.Contains(term) ||
                    (e.MiddleName != null && e.MiddleName.Contains(term)))
                .Take(20)
                .ToListAsync(ct);
        }
    }
}