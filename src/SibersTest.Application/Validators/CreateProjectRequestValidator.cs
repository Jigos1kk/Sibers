using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using SibersTest.Application.DTOs;

namespace SibersTest.Application.Validators
{
    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequestDto>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime).WithMessage("End date must be greater than start date.");

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 10).WithMessage("Priority must be between 1 and 10.");

            RuleFor(x => x.CustomerCompanyId)
                .NotEmpty().WithMessage("Customer company is required.");

            RuleFor(x => x.ContractorCompanyId)
                .NotEmpty().WithMessage("Contractor company is required.");

            RuleFor(x => x.ManagerId)
                .GreaterThan(0).WithMessage("Valid manager must be selected.");

            RuleFor(x => x.EmployeeIds)
                .NotEmpty().WithMessage("At least one employee must be assigned to the project.");
        }
    }
}