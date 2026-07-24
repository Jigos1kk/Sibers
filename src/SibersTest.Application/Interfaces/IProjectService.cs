using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Interfaces
{
    public interface IProjectService
    {
        Task<Project> CreateAsync(ProjectRequestDto request, CancellationToken ct = default);
        Task<List<Project>> ReadAsync(CancellationToken ct = default);
        Task<Project> ReadAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(int id, ProjectRequestDto request, CancellationToken ct = default);
        Task UpdateAsync(int id, CancellationToken ct = default);
    }
}