using System;
using System.Collections.Generic;
using SibersTest.Domain.Entities;

namespace SibersTest.Application.DTOs
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public string CustomerCompanyName { get; set; } = string.Empty;
        public string ContractorCompanyName { get; set; } = string.Empty;
        public Employe Manager { get; set; } = null!;
        public ICollection<Employe> Employes { get; set; } = new List<Employe>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}