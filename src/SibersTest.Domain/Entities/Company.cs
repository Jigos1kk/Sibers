using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SibersTest.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<Project> CustomerProjects { get; set; } = new List<Project>();
        public ICollection<Project> ContractorProjects { get; set; } = new List<Project>();
    }
}