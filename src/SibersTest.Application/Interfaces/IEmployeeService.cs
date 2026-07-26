using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employe> CreateAsync(EmployeeRequestDto request, CancellationToken ct = default);
        Task<List<Employe>> ReadAsync(CancellationToken ct = default);
        Task<Employe> ReadAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, EmployeeRequestDto request, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}