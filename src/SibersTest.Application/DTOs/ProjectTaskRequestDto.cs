using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SibersTest.Domain.Enum;

namespace SibersTest.Application.DTOs
{
    public class ProjectTaskRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.ToDo;
        public int AuthorId { get; set; }
        public int AssignedId { get; set; }
        public int ProjectId { get; set; }
    }
}