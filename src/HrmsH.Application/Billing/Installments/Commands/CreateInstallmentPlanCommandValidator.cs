using FluentValidation;

namespace HrmsH.Application.Billing.Installments.Commands;

public sealed class CreateInstallmentPlanCommandValidator : AbstractValidator<CreateInstallmentPlanCommand>
{
    public CreateInstallmentPlanCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Amount).GreaterThan(0);
        });
    }
}
