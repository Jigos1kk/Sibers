using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Services
{
    /// <summary>
    /// Service implementing business logic for project management.
    /// Handles project creation, retrieval, update, and deletion with associated companies and employees.
    /// All write operations are wrapped in a single transaction to ensure data consistency.
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectTaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(
            IProjectRepository projectRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository,
            IProjectTaskRepository taskRepository,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new project with the specified details within a single transaction.
        /// Automatically resolves or creates customer and contractor companies by name.
        /// Assigns the specified employees to the project.
        /// If any step fails, all changes are rolled back.
        /// </summary>
        public async Task<Project> CreateAsync(ProjectRequestDto request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
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

                await _unitOfWork.SaveChangesAsync(ct);

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
                await _unitOfWork.CommitAsync(ct);

                return await _projectRepository.GetByIdAsync(project.Id, ct,
                    p => p.Customer,
                    p => p.Contractor,
                    p => p.Manager,
                    p => p.Employes);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all projects with optional filtering and sorting.
        /// </summary>
        public async Task<List<Project>> ReadAsync(ProjectFilterQueryDto? filter, CancellationToken ct = default)
        {
            return filter == null
                ? await _projectRepository.GetAllAsync(ct,
                    p => p.Customer,
                    p => p.Contractor,
                    p => p.Manager,
                    p => p.Employes)
                : await _projectRepository.GetFilteredAsync(filter, ct);
        }

        /// <summary>
        /// Retrieves a specific project by its unique identifier.
        /// </summary>
        public async Task<Project> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _projectRepository.GetByIdAsync(id, ct,
                p => p.Customer,
                p => p.Contractor,
                p => p.Manager,
                p => p.Employes);
        }

        /// <summary>
        /// Updates an existing project with the specified changes within a single transaction.
        /// Re-resolves customer and contractor companies, and reassigns employees.
        /// </summary>
        public async Task UpdateAsync(int id, ProjectRequestDto request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var project = await _projectRepository.GetByIdAsync(id, ct,
                    p => p.Customer,
                    p => p.Contractor,
                    p => p.Manager,
                    p => p.Employes);

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

                var contractor = await _companyRepository.GetByNameAsync(request.ContractorCompanyName, ct);
                if (contractor == null)
                {
                    contractor = new Company { Name = request.ContractorCompanyName };
                    await _companyRepository.AddAsync(contractor, ct);
                }

                await _unitOfWork.SaveChangesAsync(ct);

                project.CustomerId = customer.Id;
                project.ContractorId = contractor.Id;

                project.Employes.Clear();

                foreach (var employeeId in request.EmployeeIds)
                {
                    var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
                    project.Employes.Add(employee);
                }

                await _projectRepository.UpdateAsync(project, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Deletes a project by its unique identifier within a single transaction.
        /// Also removes all associated tasks and clears employee relationships.
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var project = await _projectRepository.GetByIdAsync(id, ct,
                    p => p.Employes,
                    p => p.Tasks);
                
                if (project == null)
                    throw new KeyNotFoundException($"Project with ID {id} not found.");

                // Remove all tasks associated with this project
                foreach (var task in project.Tasks.ToList())
                {
                    await _taskRepository.DeleteAsync(task, ct);
                }

                // Clear the many-to-many relationship
                project.Employes.Clear();

                await _projectRepository.DeleteAsync(project, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}