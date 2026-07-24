using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByNameAsync(string name, CancellationToken ct);
        Task AddAsync(Company company, CancellationToken ct);
    }
}