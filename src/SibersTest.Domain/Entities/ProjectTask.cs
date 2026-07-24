using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SibersTest.Domain.Entities
{
    public class ProjectTask : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public int Priority { get; set; }
        public TaskStatus Status { get; set; }
        public int AuthorID { get; set; }
        public Employe Author { get; set; } = null!;
        public int AssignedId { get; set; }
        public Employe Assigned { get; set; } = null!;
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    } 
}