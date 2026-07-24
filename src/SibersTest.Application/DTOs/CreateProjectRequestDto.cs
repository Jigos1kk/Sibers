using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SibersTest.Application.DTOs
{
    public class CreateProjectRequestDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Priority { get; set; }
        public int CustomerCompanyId { get; set; }
        public int ContractorCompanyId { get; set; }

        public int ManagerId { get; set; }
        
        /// <summary>
        /// List of employee IDs assigned to the project.
        /// </summary>
        public List<int> EmployeeIds { get; set; } = new List<int>();
    }
}