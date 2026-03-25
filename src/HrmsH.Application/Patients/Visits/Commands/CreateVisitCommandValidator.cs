using FluentValidation;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class CreateVisitCommandValidator : AbstractValidator<CreateVisitCommand>
{
    public CreateVisitCommandValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
    }
}
