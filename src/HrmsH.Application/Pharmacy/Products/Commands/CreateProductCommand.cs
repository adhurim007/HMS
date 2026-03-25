using HrmsH.Application.Pharmacy.Products.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed record CreateProductCommand(
    string Code,
    string Name,
    string? GenericName,
    string? Strength,
    string? Unit,
    decimal? DefaultSalePrice) : IRequest<ProductDto>;
