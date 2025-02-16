using FluentValidation;
using FluentValidation.AspNetCore;
using Homespirations.Core.Entities;
using NUlid;

namespace Homespirations.Application.Validators;

public class MediaValidator : AbstractValidator<Media>
{
    public MediaValidator()
    {
        RuleFor(m => m.HomeSpaceId)
            .NotEmpty().WithMessage("HomeSpace Id is required.")
            .Must(id => Ulid.TryParse(id.ToString(), out _))
            .WithMessage("Invalid HomeSpace Id.");
        RuleFor(m => m.MediaType)
            .NotEmpty().WithMessage("Media Type is required.")
            .IsInEnum().WithMessage("Invalid Media Type.");
        RuleFor(m => m.UpdatedAt)
            .GreaterThanOrEqualTo(m => m.CreatedAt)
            .When(m => m.UpdatedAt.HasValue)
            .WithMessage("UpdatedAt must be after CreatedAt.");
    }

}
