using HrmsH.Application.Billing.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Services.Commands;

public sealed record CreateServiceItemCommand(
    string Code,
    string Name,
    decimal Price) : IRequest<ServiceItemDto>;
