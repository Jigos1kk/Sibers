using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Domain.Enum;

namespace SibersTest.Application.DTOs
{
    public class ProjectFilterQueryDto
    {
        /// <summary>
        /// Filter: Projects starting from this date.
        /// </summary>
        public DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Filter: Projects starting up to this date.
        /// </summary>
        public DateTime? StartDateTo { get; set; }

        /// <summary>
        /// Filter: Minimum priority.
        /// </summary>
        public int? PriorityFrom { get; set; }

        /// <summary>
        /// Filter: Maximum priority.
        /// </summary>
        public int? PriorityTo { get; set; }

        /// <summary>
        /// Filter: Exact or partial match for customer company name.
        /// </summary>
        public string? CustomerCompanyName { get; set; }

        public string? ContractorCompanyName { get; set; }

        /// <summary>
        /// Filter: Specific manager ID.
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// Sorting: Field to sort by. Default is StartDate.
        /// </summary>
        public ProjectSortBy SortBy { get; set; } = ProjectSortBy.StartDate;

        /// <summary>
        /// Sorting: Direction. True for descending, false for ascending. Default is descending (newest first).
        /// </summary>
        public bool IsDescending { get; set; } = true;
    }
}