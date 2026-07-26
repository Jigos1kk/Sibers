using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(
            IProjectRepository projectRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }

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

        public async Task<List<Employe>> ReadAsync(CancellationToken ct = default)
        {
            return await _employeeRepository.GetAllAsync(ct);
        }

        public async Task<Employe> ReadAsync(int id, CancellationToken ct = default)
        {
            return await _employeeRepository.GetByIdAsync(id, ct);
        }

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

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, ct);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            
            await _employeeRepository.DeleteAsync(employee, ct);
        }
    }
}