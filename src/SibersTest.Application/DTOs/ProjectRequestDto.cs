using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SibersTest.Application.DTOs
{
    public class ProjectRequestDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public string CustomerCompanyName { get; set; } = string.Empty;
        public string ContractorCompanyName { get; set; } = string.Empty;

        public int ManagerId { get; set; }
        
        /// <summary>
        /// List of employee IDs assigned to the project.
        /// </summary>
        public List<int> EmployeeIds { get; set; } = new List<int>();
    }
}