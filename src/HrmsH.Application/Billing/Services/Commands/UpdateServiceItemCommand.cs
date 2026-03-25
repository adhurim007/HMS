using HrmsH.Application.Billing.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Services.Commands;

public sealed record UpdateServiceItemCommand(
    int Id,
    string Name,
    decimal Price,
    bool IsActive) : IRequest<ServiceItemDto>;
