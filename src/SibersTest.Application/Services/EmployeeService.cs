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
    /// Service implementing business logic for employee management.
    /// Handles employee creation, retrieval, update, and deletion.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="projectRepository">Repository for project data access.</param>
        /// <param name="companyRepository">Repository for company data access.</param>
        /// <param name="employeeRepository">Repository for employee data access.</param>
        public EmployeeService(
            IProjectRepository projectRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Creates a new employee with the specified personal details.
        /// </summary>
        /// <param name="request">The employee creation data including first name, last name, middle name, and email.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created <see cref="Employe"/> entity.</returns>
        public async Task<Employe> CreateAsync(EmployeeRequestDto request, CancellationToken ct = default)
        {
            var employee = new Employe
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email
            };

            await _employeeRepository.AddAsync(employee, ct);
            return employee;
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of all <see cref="Employe"/> entities.</returns>
        public async Task<List<Employe>> ReadAsync(CancellationToken ct = default)
        {
            return await _employeeRepository.GetAllAsync(ct);
        }

        /// <summary>
        /// Retrieves a specific employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Employe"/> entity.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the employee is not found.</exception>
        public async Task<Employe> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _employeeRepository.GetByIdAsync(id, ct);
        }

        /// <summary>
        /// Searches employees by name (first name, last name, or middle name).
        /// </summary>
        /// <param name="term">The search term to filter employees by.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of matching <see cref="Employe"/> entities.</returns>
        public async Task<List<Employe>> SearchAsync(string term, CancellationToken ct = default)
        {
            return await _employeeRepository.SearchAsync(term, ct);
        }

        /// <summary>
        /// Updates an existing employee's personal details.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to update.</param>
        /// <param name="request">The updated employee data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the employee is not found.</exception>
        public async Task UpdateAsync(int id, EmployeeRequestDto request, CancellationToken ct = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, ct);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.MiddleName = request.MiddleName;
            employee.Email = request.Email;

            await _employeeRepository.UpdateAsync(employee, ct);
        }

        /// <summary>
        /// Deletes an employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the employee is not found.</exception>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, ct);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            
            await _employeeRepository.DeleteAsync(employee, ct);
        }
    }
}