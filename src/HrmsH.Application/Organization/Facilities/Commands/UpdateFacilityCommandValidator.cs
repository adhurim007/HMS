using FluentValidation;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).MaximumLength(50);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId.HasValue);
        RuleFor(x => x)
            .Must(x => !x.ParentId.HasValue || x.ParentId.Value != x.Id)
            .WithMessage("Facility cannot be its own parent.");
    }
}

