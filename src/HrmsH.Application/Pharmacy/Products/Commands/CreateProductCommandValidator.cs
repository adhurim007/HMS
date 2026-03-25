using FluentValidation;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DefaultSalePrice).GreaterThanOrEqualTo(0).When(x => x.DefaultSalePrice.HasValue);
    }
}
