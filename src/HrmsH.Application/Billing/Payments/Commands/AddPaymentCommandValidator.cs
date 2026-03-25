using FluentValidation;

namespace HrmsH.Application.Billing.Payments.Commands;

public sealed class AddPaymentCommandValidator : AbstractValidator<AddPaymentCommand>
{
    public AddPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).MaximumLength(50);
        RuleFor(x => x.Reference).MaximumLength(200);
    }
}
