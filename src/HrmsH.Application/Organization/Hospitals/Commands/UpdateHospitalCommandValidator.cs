using FluentValidation;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed class UpdateHospitalCommandValidator : AbstractValidator<UpdateHospitalCommand>
{
    public UpdateHospitalCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).MaximumLength(50);
        RuleFor(x => x.Address).MaximumLength(500);
    }
}
