using HrmsH.Application.Billing.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Services.Queries;

public sealed record GetServiceItemByIdQuery(int Id) : IRequest<ServiceItemDto?>;
