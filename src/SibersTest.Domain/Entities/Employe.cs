using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace SibersTest.Domain.Entities
{
    public class Employe : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ProjectTask> AuthoredTasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
    }
}