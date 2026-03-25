using MediatR;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed record DeleteFacilityCommand(int Id) : IRequest;

