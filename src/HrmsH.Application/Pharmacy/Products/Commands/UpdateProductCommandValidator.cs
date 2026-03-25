using FluentValidation;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DefaultSalePrice).GreaterThanOrEqualTo(0).When(x => x.DefaultSalePrice.HasValue);
    }
}
