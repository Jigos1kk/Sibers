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
    /// All write operations are wrapped in a single transaction to ensure data consistency.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new employee with the specified personal details within a single transaction.
        /// </summary>
        public async Task<Employe> CreateAsync(EmployeeRequestDto request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var employee = new Employe
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleName = request.MiddleName,
                    Email = request.Email
                };

                await _employeeRepository.AddAsync(employee, ct);
                await _unitOfWork.CommitAsync(ct);
                return employee;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        public async Task<List<Employe>> ReadAsync(CancellationToken ct = default)
        {
            return await _employeeRepository.GetAllAsync(ct);
        }

        /// <summary>
        /// Retrieves a specific employee by their unique identifier.
        /// </summary>
        public async Task<Employe> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _employeeRepository.GetByIdAsync(id, ct);
        }

        /// <summary>
        /// Searches employees by name (first name, last name, or middle name).
        /// </summary>
        public async Task<List<Employe>> SearchAsync(string term, CancellationToken ct = default)
        {
            return await _employeeRepository.SearchAsync(term, ct);
        }

        /// <summary>
        /// Updates an existing employee's personal details within a single transaction.
        /// </summary>
        public async Task UpdateAsync(int id, EmployeeRequestDto request, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id, ct);

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {id} not found.");

                employee.FirstName = request.FirstName;
                employee.LastName = request.LastName;
                employee.MiddleName = request.MiddleName;
                employee.Email = request.Email;

                await _employeeRepository.UpdateAsync(employee, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Deletes an employee by their unique identifier within a single transaction.
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id, ct);

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {id} not found.");
                
                await _employeeRepository.DeleteAsync(employee, ct);
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