using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SibersTest.Application.DTOs;
using SibersTest.Application.Interfaces;
using SibersTest.Domain.Entities;
using SibersTest.Domain.Enum;

namespace SibersTest.Data.Repositories
{
    /// <summary>
    /// Repository for managing project data persistence.
    /// Implements CRUD operations and advanced filtering/sorting for projects.
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new project to the database.
        /// </summary>
        /// <param name="project">The project entity to add.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task AddAsync(Project project, CancellationToken ct)
        {
            await _context.Projects.AddAsync(project, ct);
            await _context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Updates an existing project in the database.
        /// </summary>
        /// <param name="project">The project entity with updated values.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task UpdateAsync(Project project, CancellationToken ct)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Retrieves all projects with included related entities (Customer, Contractor, Manager, Employees).
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of all <see cref="Project"/> entities.</returns>
        public async Task<List<Project>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Contractor)
                .Include(p => p.Manager)
                .Include(p => p.Employes)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Retrieves all projects with included related entities (intended for filtering scenarios).
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of all <see cref="Project"/> entities.</returns>
        public async Task<List<Project>> GetAllFillterAsync(CancellationToken ct)
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Contractor)
                .Include(p => p.Manager)
                .Include(p => p.Employes)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Retrieves a specific project by its unique identifier with all related entities included.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Project"/> entity.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the project is not found.</exception>
        public async Task<Project> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Contractor)
                .Include(p => p.Manager)
                .Include(p => p.Employes)
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new InvalidOperationException($"Not found project with ID {id}");
        }

        /// <summary>
        /// Deletes a project from the database.
        /// </summary>
        /// <param name="project">The project entity to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        public async Task DeleteAsync(Project project, CancellationToken ct)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Retrieves projects with advanced filtering and sorting capabilities.
        /// Supports filtering by date range, priority range, customer company name, and manager ID.
        /// Supports sorting by name, start date, end date, or priority in ascending or descending order.
        /// </summary>
        /// <param name="filter">The filter and sort parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A filtered and sorted list of <see cref="Project"/> entities.</returns>
        public async Task<List<Project>> GetFillterAsync(ProjectFilterQueryDto filter, CancellationToken cancellationToken)
        {
            var query = _context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Contractor)
                .Include(p => p.Manager)
                .Include(p => p.Employes)
                .AsQueryable();

            if (filter.StartDateFrom.HasValue)
            {
                query = query.Where(p => p.StartDate >= filter.StartDateFrom.Value);
            }

            if (filter.StartDateTo.HasValue)
            {
                query = query.Where(p => p.StartDate <= filter.StartDateTo.Value);
            }

            if (filter.PriorityFrom.HasValue)
            {
                query = query.Where(p => p.Priority >= filter.PriorityFrom.Value);
            }

            if (filter.PriorityTo.HasValue)
            {
                query = query.Where(p => p.Priority <= filter.PriorityTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CustomerCompanyName))
            {
                var companyNameLower = filter.CustomerCompanyName.ToLower();
                query = query.Where(p => p.Customer.Name.ToLower().Contains(companyNameLower));
            }

            if (filter.ManagerId.HasValue)
            {
                query = query.Where(p => p.ManagerId == filter.ManagerId.Value);
            }

            query = filter.SortBy switch
            {
                ProjectSortBy.Name => filter.IsDescending 
                    ? query.OrderByDescending(p => p.Name) 
                    : query.OrderBy(p => p.Name),
                    
                ProjectSortBy.EndDate => filter.IsDescending 
                    ? query.OrderByDescending(p => p.EndDate) 
                    : query.OrderBy(p => p.EndDate),
                    
                ProjectSortBy.Priority => filter.IsDescending 
                    ? query.OrderByDescending(p => p.Priority) 
                    : query.OrderBy(p => p.Priority),
                    
                _ => filter.IsDescending 
                    ? query.OrderByDescending(p => p.StartDate) 
                    : query.OrderBy(p => p.StartDate)
            };

            return await query.ToListAsync(cancellationToken);
        }
    }
}