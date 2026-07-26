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
                var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
                project.Employes.Add(employee);
            }

            await _projectRepository.AddAsync(project, ct);

            return project;
        }

        public async Task<List<Project>> ReadAsync(ProjectFilterQueryDto? filter, CancellationToken ct = default)
        {
            
            return filter == null
                ? await _projectRepository.GetAllAsync(ct)
                : await _projectRepository.GetFillterAsync(filter, ct);
        }

        public async Task<Project> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _projectRepository.GetByIdAsync(id, ct);
        }

        public async Task UpdateAsync(int id, ProjectRequestDto request, CancellationToken ct = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, ct);

            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }

            project.Name = request.ProjectName;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.Priority = request.Priority;
            project.ManagerId = request.ManagerId;

            var customer = await _companyRepository.GetByNameAsync(request.CustomerCompanyName, ct);
            if (customer == null)
            {
                customer = new Company { Name = request.CustomerCompanyName };
                await _companyRepository.AddAsync(customer, ct);
            }
            project.CustomerId = customer.Id;

            var contractor = await _companyRepository.GetByNameAsync(request.ContractorCompanyName, ct);
            if (contractor == null)
            {
                contractor = new Company { Name = request.CustomerCompanyName };
                await _companyRepository.AddAsync(contractor, ct);
            }
            project.ContractorId = contractor.Id;

            project.Employes.Clear();

            foreach (var employeeId in request.EmployeeIds)
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
                project.Employes.Add(employee);
            }

            await _projectRepository.UpdateAsync(project, ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, ct);
            
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {id} not found.");

            await _projectRepository.DeleteAsync(project, ct);
        }
    }
}