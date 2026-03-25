using FluentValidation;

namespace HrmsH.Application.Pharmacy.Sales.Commands;

public sealed class CreatePharmacySaleCommandValidator : AbstractValidator<CreatePharmacySaleCommand>
{
    public CreatePharmacySaleCommandValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.Items).NotNull().NotEmpty();

        RuleForEach(x => x.Items).ChildRules(i =>
        {
            i.RuleFor(x => x.ProductId).GreaterThan(0);
            i.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}

