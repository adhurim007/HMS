using FluentValidation;

namespace HrmsH.Application.Pharmacy.Purchases.Commands;

public sealed class CreatePurchaseInvoiceCommandValidator
    : AbstractValidator<CreatePurchaseInvoiceCommand>
{
    public CreatePurchaseInvoiceCommandValidator()
    {
        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Items).NotNull().NotEmpty();

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPurchasePrice).GreaterThanOrEqualTo(0);
            item.RuleFor(i => i.ExpiryDate).NotEmpty();
        });
    }
}

