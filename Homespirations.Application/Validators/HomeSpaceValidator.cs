using FluentValidation;
using Homespirations.Core.Entities;

namespace Homespirations.Application.Validators;

public class HomeSpaceValidator : AbstractValidator<HomeSpace>
{
    public HomeSpaceValidator()
    {
        RuleFor(h => h.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
        RuleFor(h => h.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}