using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SibersTest.Domain.Entities;

namespace SibersTest.Data.Configurations
{
    public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.HasOne(t => t.Author)
                .WithMany(e => e.AuthoredTasks)
                .HasForeignKey(t => t.AuthorID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Assigned)
                .WithMany(e => e.AssignedTasks)
                .HasForeignKey(t => t.AssignedId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}