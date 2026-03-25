using FluentValidation;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class UpdateVisitCommandValidator : AbstractValidator<UpdateVisitCommand>
{
    public UpdateVisitCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
