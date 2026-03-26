using FluentValidation;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class CreateFacilityCommandValidator : AbstractValidator<CreateFacilityCommand>
{
    public CreateFacilityCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).MaximumLength(50);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId.HasValue);
    }
}

