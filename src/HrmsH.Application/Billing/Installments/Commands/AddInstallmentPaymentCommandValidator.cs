using FluentValidation;

namespace HrmsH.Application.Billing.Installments.Commands;

public sealed class AddInstallmentPaymentCommandValidator : AbstractValidator<AddInstallmentPaymentCommand>
{
    public AddInstallmentPaymentCommandValidator()
    {
        RuleFor(x => x.InstallmentItemId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).MaximumLength(50);
        RuleFor(x => x.Reference).MaximumLength(200);
    }
}
