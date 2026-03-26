using FluentValidation;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed class CreateHospitalCommandValidator : AbstractValidator<CreateHospitalCommand>
{
    public CreateHospitalCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).MaximumLength(50);
        RuleFor(x => x.Address).MaximumLength(500);
    }
}
