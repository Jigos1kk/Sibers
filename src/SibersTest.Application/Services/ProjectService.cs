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
    /// <summary>
    /// Service implementing business logic for project management.
    /// Handles project creation, retrieval, update, and deletion with associated companies and employees.
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="projectRepository">Repository for project data access.</param>
        /// <param name="companyRepository">Repository for company data access.</param>
        /// <param name="employeeRepository">Repository for employee data access.</param>
        public ProjectService(
            IProjectRepository projectRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Creates a new project with the specified details.
        /// Automatically resolves or creates customer and contractor companies by name.
        /// Assigns the specified employees to the project.
        /// </summary>
        /// <param name="request">The project creation data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created <see cref="Project"/> entity.</returns>
        public async Task<Project> CreateAsync(ProjectRequestDto request, CancellationToken ct = default)
        {
            var customer = await _companyRepository.GetByNameAsync(request.CustomerCompanyName, ct);
            System.Console.WriteLine(customer);
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

            return await _projectRepository.GetByIdAsync(project.Id, ct);
        }

        /// <summary>
        /// Retrieves all projects with optional filtering and sorting.
        /// </summary>
        /// <param name="filter">Optional filter and sort parameters. If null, returns all projects.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of <see cref="Project"/> entities.</returns>
        public async Task<List<Project>> ReadAsync(ProjectFilterQueryDto? filter, CancellationToken ct = default)
        {
            
            return filter == null
                ? await _projectRepository.GetAllAsync(ct)
                : await _projectRepository.GetFillterAsync(filter, ct);
        }

        /// <summary>
        /// Retrieves a specific project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Project"/> entity.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the project is not found.</exception>
        public async Task<Project> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _projectRepository.GetByIdAsync(id, ct);
        }

        /// <summary>
        /// Updates an existing project with the specified changes.
        /// Re-resolves customer and contractor companies, and reassigns employees.
        /// </summary>
        /// <param name="id">The unique identifier of the project to update.</param>
        /// <param name="request">The updated project data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the project is not found.</exception>
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
                contractor = new Company { Name = request.ContractorCompanyName };
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

        /// <summary>
        /// Deletes a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the project is not found.</exception>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var project = await _projectRepository.GetByIdAsync(id, ct);
            
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {id} not found.");

            await _projectRepository.DeleteAsync(project, ct);
        }
    }
}