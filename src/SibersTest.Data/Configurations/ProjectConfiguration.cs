using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SibersTest.Domain.Entities;
using SQLitePCL;

namespace SibersTest.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasOne(p => p.Customer)
                .WithMany(c => c.CustomerProjects)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Contractor)
                .WithMany(c => c.ContractorProjects)
                .HasForeignKey(c => c.ContractorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Manager)
                .WithMany(e => e.ManagedProjects)
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(p => p.Employes)
                .WithMany(e => e.Projects)
                .UsingEntity(j => j.ToTable("ProjectEmployees"));
        }
    }
}