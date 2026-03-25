using HrmsH.Application.Pharmacy.Products.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed record UpdateProductCommand(
    int Id,
    string Name,
    string? GenericName,
    string? Strength,
    string? Unit,
    decimal? DefaultSalePrice,
    bool IsActive) : IRequest<ProductDto>;
