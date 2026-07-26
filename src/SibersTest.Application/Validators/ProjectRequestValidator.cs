using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using SibersTest.Application.DTOs;

namespace SibersTest.Application.Validators
{
    public class ProjectRequestValidator : AbstractValidator<ProjectRequestDto>
    {
        public ProjectRequestValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.");

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 100).WithMessage("Priority must be between 1 and 100.");

            RuleFor(x => x.CustomerCompanyName)
                .NotEmpty().WithMessage("Customer name company is required.");

            RuleFor(x => x.ContractorCompanyName)
                .NotEmpty().WithMessage("Contractor name company is required.");

            RuleFor(x => x.ManagerId)
                .GreaterThan(0).WithMessage("Valid manager must be selected.");

            RuleFor(x => x.EmployeeIds)
                .NotEmpty().WithMessage("At least one employee must be assigned to the project.");
        }
    }
}