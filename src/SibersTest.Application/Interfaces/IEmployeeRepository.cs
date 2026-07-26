using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employe> GetByIdAsync(int id, CancellationToken ct);
        Task<List<Employe>> GetAllAsync(CancellationToken ct);
        Task<List<Employe>> SearchAsync(string term, CancellationToken ct);
        Task AddAsync(Employe employee, CancellationToken ct);
        Task UpdateAsync(Employe employee, CancellationToken ct);
        Task DeleteAsync(Employe employee, CancellationToken ct);
    }
}