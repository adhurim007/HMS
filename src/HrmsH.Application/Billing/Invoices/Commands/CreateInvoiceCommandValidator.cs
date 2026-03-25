using FluentValidation;

namespace HrmsH.Application.Billing.Invoices.Commands;

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500)
                .When(x =>
                    (!x.VisitServiceId.HasValue || x.VisitServiceId == 0) &&
                    (!x.LaboratoryOrderItemId.HasValue || x.LaboratoryOrderItemId == 0));
            item.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
            item.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}
