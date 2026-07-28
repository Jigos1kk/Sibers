using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Company?> GetByNameAsync(string name, CancellationToken ct)
        {
            var normalizedName = name?.ToUpper();
            if (string.IsNullOrEmpty(normalizedName)) return null;

            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name.ToUpper() == normalizedName, ct);
        }
    }
}