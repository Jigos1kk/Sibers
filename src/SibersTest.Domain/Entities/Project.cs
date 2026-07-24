using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SibersTest.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public int CustomerId { get; set; }
        public Company Customer { get; set; } = null!;
        public int ContractorId { get; set; }
        public Company Contractor { get; set; } = null!;
        public int ManagerId { get; set; }
        public Employe Manager { get; set; } = null!;
        public ICollection<Employe> Employes { get; set; } = new List<Employe>();
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}