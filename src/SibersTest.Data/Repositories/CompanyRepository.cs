using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Company company, CancellationToken ct)
        {
            await _context.Companies.AddAsync(company, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Company?> GetByNameAsync(string name, CancellationToken ct)
        {
            var normalizedName = name?.ToUpper();
            if (string.IsNullOrEmpty(normalizedName)) return null;

            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Name.ToUpper() == normalizedName, ct);
        }
    }
}