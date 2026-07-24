using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Application.Validators;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Project> CreateAsync(ProjectRequestDto request, CancellationToken ct = default)
        {
            var customer = await _companyRepository.GetByNameAsync(request.CustomerCompanyName, ct);
            if (customer == null)
            {
                customer = new Company { Name = request.CustomerCompanyName };
                await _companyRepository.AddAsync(customer, ct);
            }

            var contractor = await _companyRepository.GetByNameAsync(request.ContractorCompanyName, ct);
            if (contractor == null)
            {
                contractor = new Company { Name = request.ContractorCompanyName };
                await _companyRepository.AddAsync(contractor, ct);
            }

            var project = new Project
            {
                Name = request.ProjectName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Priority = request.Priority,
                CustomerId = customer.Id,
                ContractorId = contractor.Id,
                ManagerId = request.ManagerId
            };

            foreach (var employeeId in request.EmployeeIds)
            {
                project.Employes.Add(new Employe { Id = employeeId });
            }

            await _projectRepository.AddAsync(project, ct);

            return project;
        }

        public async Task<List<Project>> ReadAsync(CancellationToken ct = default)
        {
            return await _projectRepository.GetAllAsync(ct);
        }

        public async Task<Project> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _projectRepository.GetByIdAsync(id, ct);
        }

        public Task UpdateAsync(int id, ProjectRequestDto request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}