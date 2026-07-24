using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;

namespace SibersTest.Application.Interfaces
{
    public interface IProjectService
    {
        Task CreateAsync(CreateProjectRequestDto project, CancellationToken ct);
    }
}