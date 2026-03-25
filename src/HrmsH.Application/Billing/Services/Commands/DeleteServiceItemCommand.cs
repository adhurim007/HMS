using MediatR;

namespace HrmsH.Application.Billing.Services.Commands;

public sealed record DeleteServiceItemCommand(int Id) : IRequest;
