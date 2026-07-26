using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SibersTest.Application.DTOs;

namespace SibersTest.Application.Validators
{
    public class ProjectTaskRequestValidator : AbstractValidator<ProjectTaskRequestDto>
    {
        public ProjectTaskRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Task name is required.")
                .MaximumLength(200).WithMessage("Task name cannot exceed 200 characters.");

            RuleFor(x => x.Priority)
                .InclusiveBetween(0, 100).WithMessage("Priority must be between 0 and 100.");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Valid author must be selected.");

            RuleFor(x => x.AssignedId)
                .GreaterThan(0).WithMessage("Valid assigned employee must be selected.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Valid project must be selected.");
        }
    }
}