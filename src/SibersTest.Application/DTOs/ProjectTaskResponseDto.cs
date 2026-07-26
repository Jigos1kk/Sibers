using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Domain.Enum;

namespace SibersTest.Application.DTOs
{
    public class ProjectTaskResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public EmployeeResponseDto Author { get; set; } = null!;
        public EmployeeResponseDto Assigned { get; set; } = null!;
        public int ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}