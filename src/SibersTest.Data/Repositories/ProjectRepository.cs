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
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves projects with advanced filtering and sorting capabilities.
        /// Supports filtering by date range, priority range, customer company name, and manager ID.
        /// Supports sorting by name, start date, end date, or priority in ascending or descending order.
        /// </summary>
        public async Task<List<Project>> GetFilteredAsync(ProjectFilterQueryDto filter, CancellationToken cancellationToken)
        {
            var query = _dbSet
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