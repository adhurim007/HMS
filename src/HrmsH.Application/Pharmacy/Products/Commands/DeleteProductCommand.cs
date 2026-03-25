using MediatR;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed record DeleteProductCommand(int Id) : IRequest;
