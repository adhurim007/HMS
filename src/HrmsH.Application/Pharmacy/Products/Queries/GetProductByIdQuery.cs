using HrmsH.Application.Pharmacy.Products.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Products.Queries;

public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
