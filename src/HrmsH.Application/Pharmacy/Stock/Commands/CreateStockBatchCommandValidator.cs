using FluentValidation;

namespace HrmsH.Application.Pharmacy.Stock.Commands;

public sealed class CreateStockBatchCommandValidator : AbstractValidator<CreateStockBatchCommand>
{
    public CreateStockBatchCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
    }
}
